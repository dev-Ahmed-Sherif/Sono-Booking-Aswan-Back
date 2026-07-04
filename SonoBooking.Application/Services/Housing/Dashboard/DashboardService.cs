using Microsoft.EntityFrameworkCore;
using SonoBooking.Application.Services.Housing.Availability;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Housing.Dashboard;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using SonoBooking.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Dashboard;

public class DashboardService(
    IResponseResult responseResult,
    SonoBookingDbContext dbContext,
    IUnitOccupancyService unitOccupancyService) : IDashboardService
{
    private const int ChartDays = 365;

    public async Task<IFinalResult> GetGovernorSummaryAsync(CancellationToken cancellationToken = default)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        DateTime todayStartUtc = today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        DateTime todayEndUtc = todayStartUtc.AddDays(1);
        DateOnly chartStart = today.AddDays(-(ChartDays - 1));
        DateTime chartStartUtc = chartStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        List<(DateOnly RequestDay, Status Status)> requestsInChartRange = [.. (await dbContext.Requests
            .AsNoTracking()
            .Where(r => !r.IsDeleted
                && r.RequestDate >= chartStartUtc
                && r.RequestDate < todayEndUtc)
            .Select(r => new { r.RequestDate, r.Status })
            .ToListAsync(cancellationToken))
            .Select(r => (DateOnly.FromDateTime(r.RequestDate.Date), r.Status))];

        List<ReservationChartRow> reservations = await dbContext.Reservations
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .Select(r => new ReservationChartRow
            {
                ReservationId = r.Id,
                RequestId = r.RequestId,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                ActualCheckOutDay = r.ActualCheckOutDate.HasValue
                    ? DateOnly.FromDateTime(r.ActualCheckOutDate.Value.Date)
                    : null,
                Status = r.Status
            })
            .ToListAsync(cancellationToken);

        List<PaymentChartRow> paymentsInChartRange = [.. (await dbContext.Payments
            .AsNoTracking()
            .Where(p => !p.IsDeleted
                && p.PaymentStatus != PaymentStatus.Failed
                && p.PaymentStatus != PaymentStatus.Refunded
                && p.PaymentDate >= chartStartUtc
                && p.PaymentDate < todayEndUtc)
            .Select(p => new { p.PaymentDate, p.Amount })
            .ToListAsync(cancellationToken))
            .Select(p => new PaymentChartRow
            {
                PaymentDay = DateOnly.FromDateTime(p.PaymentDate.Date),
                Amount = p.Amount
            })];

        Dictionary<string, string?> previousRequestById = await dbContext.Requests
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .Select(r => new { r.Id, r.PreviousRequestId })
            .ToDictionaryAsync(
                r => r.Id,
                r => r.PreviousRequestId,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        List<DashboardOccupancyCache.ApprovedRequestRow> approvedRequests = await dbContext.Requests
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.Status == Status.Approved)
            .Select(r => new DashboardOccupancyCache.ApprovedRequestRow
            {
                Id = r.Id,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                PreviousRequestId = r.PreviousRequestId,
                RequestCatagory = r.RequestCatagory
            })
            .ToListAsync(cancellationToken);

        List<DashboardOccupancyCache.ExtensionEndRow> approvedExtensions = await dbContext.Extensions
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.Status == Status.Approved)
            .Select(e => new DashboardOccupancyCache.ExtensionEndRow
            {
                ReservationId = e.ReservationId,
                EndDate = e.EndDate
            })
            .ToListAsync(cancellationToken);

        List<DashboardOccupancyCache.ReservationEndRow> reservationEndRows = [.. reservations
            .Select(r => new DashboardOccupancyCache.ReservationEndRow
            {
                ReservationId = r.ReservationId,
                RequestId = r.RequestId,
                EffectiveEndDay = r.ActualCheckOutDay ?? r.EndDate,
                Status = r.Status
            })];

        List<RequestUnit> requestUnits = await dbContext.RequestUnits
            .AsNoTracking()
            .Where(ru => !ru.IsDeleted)
            .ToListAsync(cancellationToken);

        DashboardOccupancyCache occupancyCache = DashboardOccupancyCache.Create(
            reservationEndRows,
            approvedExtensions,
            approvedRequests,
            previousRequestById,
            requestUnits);

        DashboardOccupancyCalculator.HousingStructure housingStructure = await LoadHousingStructureAsync(cancellationToken);

        List<DashboardDailyStatDto> dailyStats = [];
        for (int offset = 0; offset < ChartDays; offset++)
        {
            DateOnly day = chartStart.AddDays(offset);

            int dayTotalRequests = requestsInChartRange.Count(r => r.RequestDay == day);
            int dayApproved = requestsInChartRange.Count(r =>
                r.RequestDay == day && r.Status == Status.Approved);
            int dayRejected = requestsInChartRange.Count(r =>
                r.RequestDay == day && r.Status == Status.Rejected);

            decimal dayRevenue = paymentsInChartRange
                .Where(p => p.PaymentDay == day)
                .Sum(p => p.Amount);

            (int overallPercent, List<ApartmentOccupancyItemDto> apartmentOccupancy) =
                DashboardOccupancyCalculator.CalculateForDay(
                    day,
                    occupancyCache,
                    housingStructure,
                    unitOccupancyService);

            dailyStats.Add(new DashboardDailyStatDto
            {
                Date = day.ToString("yyyy-MM-dd"),
                TotalRequests = dayTotalRequests,
                ApprovedRequests = dayApproved,
                RejectedRequests = dayRejected,
                TotalRevenue = dayRevenue,
                OccupancyPercent = overallPercent,
                ApartmentOccupancy = apartmentOccupancy
            });
        }

        DashboardDailyStatDto todayStats = dailyStats.LastOrDefault() ?? new DashboardDailyStatDto();

        List<ApprovedRequestItemDto> latestApproved = await dbContext.Requests
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.Status == Status.Approved)
            .OrderByDescending(r => r.ApprovedAt ?? r.RequestDate)
            .Take(10)
            .Select(r => new ApprovedRequestItemDto
            {
                Id = r.RequestNumber,
                Name = r.User != null ? r.User.FullName : string.Empty,
                Reason = r.RequestType != null ? r.RequestType.NameAr : string.Empty,
                Date = (r.ApprovedAt ?? r.RequestDate).ToString("yyyy-MM-dd")
            })
            .ToListAsync(cancellationToken);

        GovernorDashboardDto summary = new()
        {
            TodayTotalRequests = todayStats.TotalRequests,
            TodayApprovedRequests = todayStats.ApprovedRequests,
            TodayRejectedRequests = todayStats.RejectedRequests,
            OccupancyPercent = todayStats.OccupancyPercent,
            TotalRevenue = todayStats.TotalRevenue,
            ApartmentOccupancy = todayStats.ApartmentOccupancy,
            LatestApprovedRequests = latestApproved,
            DailyStats = dailyStats
        };

        return responseResult.PostResult(
            summary,
            HttpStatusCode.OK,
            null,
            MessagesConstants.Success);
    }

    private async Task<DashboardOccupancyCalculator.HousingStructure> LoadHousingStructureAsync(
        CancellationToken cancellationToken)
    {
        List<DashboardOccupancyCalculator.ApartmentStructureRow> apartments = await dbContext.Apartments
            .AsNoTracking()
            .Where(a => !a.IsDeleted)
            .OrderBy(a => a.ApartmentNumber)
            .Select(a => new DashboardOccupancyCalculator.ApartmentStructureRow
            {
                Id = a.Id,
                ApartmentNumber = a.ApartmentNumber,
                Rooms = a.Rooms
                    .Where(room => !room.IsDeleted)
                    .Select(room => new DashboardOccupancyCalculator.RoomStructureRow
                    {
                        Id = room.Id,
                        ApartmentId = room.ApartmentId,
                        Status = room.Status,
                        Beds = room.Beds
                            .Where(bed => !bed.IsDeleted)
                            .Select(bed => new DashboardOccupancyCalculator.BedStructureRow
                            {
                                Id = bed.Id,
                                RoomId = bed.RoomId,
                                Status = bed.Status
                            })
                            .ToList()
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new DashboardOccupancyCalculator.HousingStructure
        {
            Apartments = apartments
        };
    }

    private sealed class ReservationChartRow
    {
        public string? ReservationId { get; init; }
        public required string RequestId { get; init; }
        public required DateOnly StartDate { get; init; }
        public required DateOnly EndDate { get; init; }
        public DateOnly? ActualCheckOutDay { get; init; }
        public required ReservationStatus Status { get; init; }
    }

    private sealed class PaymentChartRow
    {
        public required DateOnly PaymentDay { get; init; }
        public required decimal Amount { get; init; }
    }
}
