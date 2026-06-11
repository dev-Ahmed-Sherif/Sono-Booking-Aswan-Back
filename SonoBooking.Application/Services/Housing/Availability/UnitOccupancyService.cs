using Microsoft.EntityFrameworkCore;
using SonoBooking.Domain;
using SonoBooking.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Availability;

public class UnitOccupancyService(SonoBookingDbContext dbContext) : IUnitOccupancyService
{
    /// <summary>
    /// Inquiry start is 12:00:01 on the selected day; blocking ends at 12:00 on ActualCheckOutDate.
    /// Checkout 21-06-2026 → bookable from 21-06-2026 12:00:01 (same calendar day).
    /// </summary>
    public bool IsUnitFreeOnInquiryStart(DateOnly inquiryStart, DateOnly? blockingEnd) =>
        !blockingEnd.HasValue ||
        inquiryStart.ToDateTime(new TimeOnly(12, 0, 1)) >
        blockingEnd.Value.ToDateTime(new TimeOnly(12, 0, 0));

    public async Task<UnitBlockingEndIndex> BuildBlockingEndIndexAsync(
        CancellationToken cancellationToken = default)
    {
        var index = new UnitBlockingEndIndex();

        var reservationCheckouts = await dbContext.Reservations
            .AsNoTracking()
            .Where(r => !r.IsDeleted
                && r.Status != ReservationStatus.Canceled
                && r.Status != ReservationStatus.NoShow)
            .Select(r => new { r.RequestId, r.ActualCheckOutDate, r.EndDate })
            .ToListAsync(cancellationToken);

        var requestEndById = new Dictionary<string, DateOnly>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in reservationCheckouts)
        {
            if (string.IsNullOrWhiteSpace(row.RequestId)) continue;
            var endYmd = row.ActualCheckOutDate is not null
                ? DateOnly.FromDateTime(row.ActualCheckOutDate.Value.Date)
                : row.EndDate;
            var key = row.RequestId.Trim();
            if (requestEndById.TryGetValue(key, out var current))
            {
                if (endYmd > current) requestEndById[key] = endYmd;
            }
            else
            {
                requestEndById[key] = endYmd;
            }
        }

        var requestUnits = await dbContext.RequestUnits
            .AsNoTracking()
            .Include(ru => ru.Bed)
            .ThenInclude(b => b.Room)
            .ToListAsync(cancellationToken);

        foreach (var unit in requestUnits)
        {
            if (string.IsNullOrWhiteSpace(unit.RequestId)) continue;
            if (!requestEndById.TryGetValue(unit.RequestId, out var endYmd)) continue;

            if (!string.IsNullOrWhiteSpace(unit.BedId))
            {
                UnitBlockingEndIndex.SetMax(index.Beds, unit.BedId, endYmd);
            }

            var roomId = unit.RoomId;
            if (string.IsNullOrWhiteSpace(roomId) && unit.Bed?.RoomId is { Length: > 0 } bedRoomId)
            {
                roomId = bedRoomId;
            }

            if (!string.IsNullOrWhiteSpace(roomId))
            {
                UnitBlockingEndIndex.SetMax(index.Rooms, roomId, endYmd);
            }

            var apartmentId = unit.ApartmentId;
            if (string.IsNullOrWhiteSpace(apartmentId) && unit.Bed?.Room?.ApartmentId is { Length: > 0 } aptFromBed)
            {
                apartmentId = aptFromBed;
            }

            if (!string.IsNullOrWhiteSpace(apartmentId))
            {
                UnitBlockingEndIndex.SetMax(index.Apartments, apartmentId, endYmd);
            }
        }

        return index;
    }

    public async Task<IReadOnlyDictionary<string, string>> GetRoomApartmentIdsAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Rooms
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.ApartmentId != null && r.ApartmentId != "")
            .ToDictionaryAsync(r => r.Id, r => r.ApartmentId, StringComparer.OrdinalIgnoreCase, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, Gender>> GetApartmentGendersAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Apartments
            .AsNoTracking()
            .Where(a => !a.IsDeleted)
            .ToDictionaryAsync(a => a.Id, a => a.Gender, StringComparer.OrdinalIgnoreCase, cancellationToken);
    }
}
