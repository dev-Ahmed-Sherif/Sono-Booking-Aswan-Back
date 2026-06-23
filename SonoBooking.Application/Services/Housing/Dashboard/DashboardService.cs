using Microsoft.EntityFrameworkCore;
using SonoBooking.Common.Constants;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Housing.Dashboard;
using SonoBooking.Domain;
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
    SonoBookingDbContext dbContext) : IDashboardService
{
    private const int ChartDays = 365;

    public async Task<IFinalResult> GetGovernorSummaryAsync(CancellationToken cancellationToken = default)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
        DateTime todayStartUtc = today.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        DateTime todayEndUtc = todayStartUtc.AddDays(1);

        List<Domain.Entities.Housing.Request> todayRequests = await dbContext.Requests
            .AsNoTracking()
            .Where(r => !r.IsDeleted
                && r.RequestDate >= todayStartUtc
                && r.RequestDate < todayEndUtc)
            .ToListAsync(cancellationToken);

        int todayTotal = todayRequests.Count;
        int todayApproved = todayRequests.Count(r => r.Status == Status.Approved);
        int todayRejected = todayRequests.Count(r => r.Status == Status.Rejected);

        HashSet<string> activeReservationRequestIds = await dbContext.Reservations
            .AsNoTracking()
            .Where(r => !r.IsDeleted
                && r.Status != ReservationStatus.Canceled
                && r.Status != ReservationStatus.NoShow
                && r.Status != ReservationStatus.Checkout
                && r.StartDate <= today
                && (r.ActualCheckOutDate.HasValue
                    ? DateOnly.FromDateTime(r.ActualCheckOutDate.Value.Date) >= today
                    : r.EndDate >= today))
            .Select(r => r.RequestId)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, cancellationToken);

        decimal totalRevenue = await dbContext.Reservations
            .AsNoTracking()
            .Where(r => !r.IsDeleted && activeReservationRequestIds.Contains(r.RequestId))
            .SumAsync(r => r.Payment!.Amount, cancellationToken);

        DateOnly chartStart = today.AddDays(-(ChartDays - 1));
        DateTime chartStartUtc = chartStart.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        List<(DateOnly RequestDay, Status Status)> requestsInChartRange = (await dbContext.Requests
            .AsNoTracking()
            .Where(r => !r.IsDeleted
                && r.RequestDate >= chartStartUtc
                && r.RequestDate < todayEndUtc)
            .Select(r => new { r.RequestDate, r.Status })
            .ToListAsync(cancellationToken))
            .Select(r => (DateOnly.FromDateTime(r.RequestDate), r.Status))
            .ToList();

        List<(DateOnly StartDate, DateOnly EndDate, DateOnly? ActualCheckOutDay, decimal Amount)> revenueReservations =
            (await dbContext.Reservations
                .AsNoTracking()
                .Where(r => !r.IsDeleted
                    && r.Payment != null
                    && r.Status != ReservationStatus.Canceled
                    && r.Status != ReservationStatus.NoShow
                    && r.Status != ReservationStatus.Checkout
                    && r.StartDate <= today
                    && (r.ActualCheckOutDate.HasValue
                        ? DateOnly.FromDateTime(r.ActualCheckOutDate.Value.Date) >= chartStart
                        : r.EndDate >= chartStart))
                .Select(r => new
                {
                    r.StartDate,
                    r.EndDate,
                    r.ActualCheckOutDate,
                    Amount = r.Payment!.Amount
                })
                .ToListAsync(cancellationToken))
            .Select(r => (
                r.StartDate,
                r.EndDate,
                r.ActualCheckOutDate.HasValue
                    ? DateOnly.FromDateTime(r.ActualCheckOutDate.Value.Date)
                    : (DateOnly?)null,
                r.Amount))
            .ToList();

        List<DashboardDailyStatDto> dailyStats = [];
        for (int offset = 0; offset < ChartDays; offset++)
        {
            DateOnly day = chartStart.AddDays(offset);

            int dayTotalRequests = requestsInChartRange.Count(r => r.RequestDay == day);
            int dayApproved = requestsInChartRange.Count(r =>
                r.RequestDay == day && r.Status == Status.Approved);
            int dayRejected = requestsInChartRange.Count(r =>
                r.RequestDay == day && r.Status == Status.Rejected);

            decimal dayRevenue = revenueReservations
                .Where(r => r.StartDate <= day
                    && (r.ActualCheckOutDay.HasValue
                        ? r.ActualCheckOutDay.Value >= day
                        : r.EndDate >= day))
                .Sum(r => r.Amount);

            dailyStats.Add(new DashboardDailyStatDto
            {
                Date = day.ToString("yyyy-MM-dd"),
                TotalRequests = dayTotalRequests,
                ApprovedRequests = dayApproved,
                RejectedRequests = dayRejected,
                TotalRevenue = dayRevenue
            });
        }

        var apartments = await dbContext.Apartments
            .AsNoTracking()
            .Where(a => !a.IsDeleted)
            .Select(a => new
            {
                a.Id,
                a.ApartmentNumber,
                Rooms = a.Rooms
                    .Where(room => !room.IsDeleted)
                    .Select(room => new
                    {
                        room.Id,
                        Beds = room.Beds
                            .Where(bed => !bed.IsDeleted)
                            .Select(bed => bed.Id)
                            .ToList()
                    })
                    .ToList()
            })
            .OrderBy(a => a.ApartmentNumber)
            .ToListAsync(cancellationToken);

        var occupiedBedIds = await dbContext.RequestUnits
            .AsNoTracking()
            .Where(ru => activeReservationRequestIds.Contains(ru.RequestId)
                && ru.BedId != null
                && ru.BedId != "")
            .Select(ru => ru.BedId!)
            .Distinct()
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, cancellationToken);

        var occupiedRoomIds = await dbContext.RequestUnits
            .AsNoTracking()
            .Where(ru => activeReservationRequestIds.Contains(ru.RequestId)
                && (ru.BedId == null || ru.BedId == "")
                && ru.RoomId != null
                && ru.RoomId != "")
            .Select(ru => ru.RoomId!)
            .Distinct()
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, cancellationToken);

        List<ApartmentOccupancyItemDto> apartmentOccupancy = [];
        int totalCapacity = 0;
        int totalOccupied = 0;

        foreach (var apartment in apartments)
        {
            int apartmentCapacity = 0;
            int apartmentOccupied = 0;

            foreach (var room in apartment.Rooms)
            {
                if (room.Beds.Count > 0)
                {
                    apartmentCapacity += room.Beds.Count;
                    apartmentOccupied += room.Beds.Count(bedId => occupiedBedIds.Contains(bedId));
                }
                else
                {
                    apartmentCapacity += 1;
                    if (occupiedRoomIds.Contains(room.Id))
                        apartmentOccupied += 1;
                }
            }

            totalCapacity += apartmentCapacity;
            totalOccupied += apartmentOccupied;

            int percent = apartmentCapacity == 0
                ? 0
                : (int)Math.Round(apartmentOccupied * 100m / apartmentCapacity, MidpointRounding.AwayFromZero);

            apartmentOccupancy.Add(new ApartmentOccupancyItemDto
            {
                UnitLabel = $"شقة {apartment.ApartmentNumber}",
                Percent = percent
            });
        }

        int occupancyPercent = totalCapacity == 0
            ? 0
            : (int)Math.Round(totalOccupied * 100m / totalCapacity, MidpointRounding.AwayFromZero);

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
            TodayTotalRequests = todayTotal,
            TodayApprovedRequests = todayApproved,
            TodayRejectedRequests = todayRejected,
            OccupancyPercent = occupancyPercent,
            TotalRevenue = totalRevenue,
            ApartmentOccupancy = apartmentOccupancy,
            LatestApprovedRequests = latestApproved,
            DailyStats = dailyStats
        };

        return responseResult.PostResult(
            summary,
            HttpStatusCode.OK,
            null,
            MessagesConstants.Success);
    }
}
