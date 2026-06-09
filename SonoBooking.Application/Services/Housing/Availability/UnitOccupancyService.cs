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
    public bool IsUnitFreeOnInquiryStart(DateOnly inquiryStart, DateOnly? blockingEnd) =>
        !blockingEnd.HasValue || inquiryStart > blockingEnd.Value;

    public async Task<UnitBlockingEndIndex> BuildBlockingEndIndexAsync(
        CancellationToken cancellationToken = default)
    {
        var index = new UnitBlockingEndIndex();

        var requestEnds = await dbContext.Requests
            .AsNoTracking()
            .Where(r => !r.IsDeleted
                && r.Status != Status.Canceled
                && r.Status != Status.NeedCompelete)
            .Select(r => new { r.Id, r.EndDate })
            .ToListAsync(cancellationToken);

        var requestEndById = requestEnds.ToDictionary(
            r => r.Id,
            r => r.EndDate,
            StringComparer.OrdinalIgnoreCase);

        var extensionEnds = await (
            from extension in dbContext.Extensions.AsNoTracking()
            join reservation in dbContext.Reservations.AsNoTracking()
                on extension.ReservationId equals reservation.Id
            where !extension.IsDeleted
                && extension.Status != Status.Canceled
                && extension.Status != Status.NeedCompelete
            select new { reservation.RequestId, extension.EndDate }
        ).ToListAsync(cancellationToken);

        foreach (var row in extensionEnds)
        {
            if (string.IsNullOrWhiteSpace(row.RequestId)) continue;
            var key = row.RequestId.Trim();
            if (requestEndById.TryGetValue(key, out var current))
            {
                if (row.EndDate > current) requestEndById[key] = row.EndDate;
            }
            else
            {
                requestEndById[key] = row.EndDate;
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
