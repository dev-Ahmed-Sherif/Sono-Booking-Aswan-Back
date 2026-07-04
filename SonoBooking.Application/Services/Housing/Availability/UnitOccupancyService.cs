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
    /// Inquiry start is 12:00:01 on the selected day (or the sent time when datetime is provided).
    /// Planned checkout: blocking ends at 12:00:00 when end date equals checkout day.
    /// Early/late checkout: blocking ends at actual checkout + 1 hour.
    /// </summary>
    public bool IsUnitFreeOnInquiryStart(DateTime inquiryStart, DateTime? blockingEnd) =>
        AvailabilityCheckoutBlocking.IsInquiryAfterBlocking(inquiryStart, blockingEnd);

    public bool IsUnitFreeForInquiryWindow(
        DateTime inquiryStartInstant,
        DateOnly inquiryStartDate,
        int nights,
        DateTime? blockingEnd,
        DateOnly? nextApprovedStart)
    {
        if (!IsUnitFreeOnInquiryStart(inquiryStartInstant, blockingEnd))
            return false;

        if (nights <= 0)
            return true;

        if (!nextApprovedStart.HasValue)
            return true;

        DateOnly inquiryEnd = inquiryStartDate.AddDays(nights);
        return inquiryEnd <= nextApprovedStart.Value;
    }

    public async Task<UnitBlockingEndIndex> BuildBlockingEndIndexAsync(
        DateOnly? inquiryStart = null,
        CancellationToken cancellationToken = default)
    {
        var index = new UnitBlockingEndIndex();

        var reservationRows = await dbContext.Reservations
            .AsNoTracking()
            .Where(r => !r.IsDeleted
                && r.Status != ReservationStatus.Canceled
                && r.Status != ReservationStatus.NoShow
                && r.ActualCheckOutDate != null)
            .Select(r => new
            {
                r.RequestId,
                r.EndDate,
                r.ActualCheckOutDate,
                r.Status
            })
            .ToListAsync(cancellationToken);

        var reservationMetaByRequestId =
            new Dictionary<string, (DateOnly EndDate, DateTime ActualCheckOut, ReservationStatus Status)>(
                StringComparer.OrdinalIgnoreCase);

        var requestEndById = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in reservationRows)
        {
            if (string.IsNullOrWhiteSpace(row.RequestId) || row.ActualCheckOutDate is null) continue;
            reservationMetaByRequestId[row.RequestId] =
                (row.EndDate, row.ActualCheckOutDate.Value, row.Status);
        }

        DateTime? inquiryStartInstant = inquiryStart.HasValue
            ? AvailabilityCheckoutBlocking.ResolveInquiryStartInstant(inquiryStart.Value)
            : null;

        var approvedRequests = await dbContext.Requests
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.Status == Status.Approved)
            .Select(r => new { r.Id, r.StartDate })
            .ToListAsync(cancellationToken);

        var approvedRequestIds = approvedRequests
            .Select(r => r.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var requestStartById = approvedRequests
            .ToDictionary(r => r.Id, r => r.StartDate, StringComparer.OrdinalIgnoreCase);

        var requestUnits = await dbContext.RequestUnits
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var unit in requestUnits)
        {
            if (string.IsNullOrWhiteSpace(unit.RequestId)) continue;
            if (!approvedRequestIds.Contains(unit.RequestId)) continue;
            if (!requestStartById.TryGetValue(unit.RequestId, out var requestStart)) continue;

            if (!string.IsNullOrWhiteSpace(unit.BedId))
            {
                UnitBlockingEndIndex.AddApprovedStart(index.BedApprovedStarts, unit.BedId, requestStart);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(unit.RoomId))
            {
                UnitBlockingEndIndex.AddApprovedStart(index.RoomApprovedStarts, unit.RoomId, requestStart);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(unit.ApartmentId))
            {
                UnitBlockingEndIndex.AddApprovedStart(index.ApartmentApprovedStarts, unit.ApartmentId, requestStart);
            }
        }

        foreach (var unit in requestUnits)
        {
            if (string.IsNullOrWhiteSpace(unit.RequestId)) continue;
            if (!requestStartById.TryGetValue(unit.RequestId, out var requestStart)) continue;
            if (!reservationMetaByRequestId.TryGetValue(unit.RequestId, out var reservationMeta)) continue;

            if (!inquiryStart.HasValue || !inquiryStartInstant.HasValue)
                continue;

            if (!AvailabilityCheckoutBlocking.TryResolveReservationBlockingEnd(
                    reservationMeta.Status,
                    requestStart,
                    reservationMeta.EndDate,
                    reservationMeta.ActualCheckOut,
                    inquiryStart.Value,
                    out var blockingEnd))
            {
                continue;
            }

            if (blockingEnd < inquiryStartInstant.Value)
                continue;

            MergeRequestEnd(requestEndById, unit.RequestId, blockingEnd);
        }

        foreach (var unit in requestUnits)
        {
            if (string.IsNullOrWhiteSpace(unit.RequestId)) continue;
            if (!requestEndById.TryGetValue(unit.RequestId, out var blockingEnd)) continue;

            if (!string.IsNullOrWhiteSpace(unit.BedId))
            {
                UnitBlockingEndIndex.SetMax(index.Beds, unit.BedId, blockingEnd);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(unit.RoomId))
            {
                UnitBlockingEndIndex.SetMax(index.Rooms, unit.RoomId, blockingEnd);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(unit.ApartmentId))
            {
                UnitBlockingEndIndex.SetMax(index.Apartments, unit.ApartmentId, blockingEnd);
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

    public async Task<HashSet<string>> GetApartmentIdsWithAvailableChildrenAsync(
        DateOnly inquiryStart,
        int nights,
        CancellationToken cancellationToken = default)
    {
        var inquiryInstant = AvailabilityCheckoutBlocking.ResolveInquiryStartInstant(inquiryStart);
        var index = await BuildBlockingEndIndexAsync(inquiryStart, cancellationToken);
        var roomApartmentById = await GetRoomApartmentIdsAsync(cancellationToken);
        var availableApartmentIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var rooms = await dbContext.Rooms
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .Select(r => new { r.Id, r.ApartmentId })
            .ToListAsync(cancellationToken);

        foreach (var room in rooms)
        {
            if (string.IsNullOrWhiteSpace(room.ApartmentId)) continue;
            var blockingEnd = index.GetRoomBlockingEnd(room.Id, room.ApartmentId);
            var nextApprovedStart = index.GetRoomNextApprovedStart(room.Id, room.ApartmentId, inquiryStart);
            if (IsUnitFreeForInquiryWindow(inquiryInstant, inquiryStart, nights, blockingEnd, nextApprovedStart))
                availableApartmentIds.Add(room.ApartmentId.Trim());
        }

        var beds = await dbContext.Beds
            .AsNoTracking()
            .Where(b => !b.IsDeleted)
            .Select(b => new { b.Id, b.RoomId })
            .ToListAsync(cancellationToken);

        foreach (var bed in beds)
        {
            if (string.IsNullOrWhiteSpace(bed.RoomId)) continue;
            if (!roomApartmentById.TryGetValue(bed.RoomId.Trim(), out var apartmentId)) continue;
            var blockingEnd = index.GetBedBlockingEnd(bed.Id, bed.RoomId, apartmentId);
            var nextApprovedStart = index.GetBedNextApprovedStart(bed.Id, bed.RoomId, apartmentId, inquiryStart);
            if (IsUnitFreeForInquiryWindow(inquiryInstant, inquiryStart, nights, blockingEnd, nextApprovedStart))
                availableApartmentIds.Add(apartmentId.Trim());
        }

        return availableApartmentIds;
    }

    public async Task<HashSet<string>> GetApartmentIdsWithBlockedChildrenAsync(
        DateOnly inquiryStart,
        int nights,
        CancellationToken cancellationToken = default)
    {
        var inquiryInstant = AvailabilityCheckoutBlocking.ResolveInquiryStartInstant(inquiryStart);
        var index = await BuildBlockingEndIndexAsync(inquiryStart, cancellationToken);
        var roomApartmentById = await GetRoomApartmentIdsAsync(cancellationToken);
        var blockedApartmentIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var rooms = await dbContext.Rooms
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .Select(r => new { r.Id, r.ApartmentId })
            .ToListAsync(cancellationToken);

        foreach (var room in rooms)
        {
            if (string.IsNullOrWhiteSpace(room.ApartmentId)) continue;
            var blockingEnd = index.GetRoomBlockingEnd(room.Id, room.ApartmentId);
            var nextApprovedStart = index.GetRoomNextApprovedStart(room.Id, room.ApartmentId, inquiryStart);
            if (!IsUnitFreeForInquiryWindow(inquiryInstant, inquiryStart, nights, blockingEnd, nextApprovedStart))
                blockedApartmentIds.Add(room.ApartmentId.Trim());
        }

        var beds = await dbContext.Beds
            .AsNoTracking()
            .Where(b => !b.IsDeleted)
            .Select(b => new { b.Id, b.RoomId })
            .ToListAsync(cancellationToken);

        foreach (var bed in beds)
        {
            if (string.IsNullOrWhiteSpace(bed.RoomId)) continue;
            if (!roomApartmentById.TryGetValue(bed.RoomId.Trim(), out var apartmentId)) continue;
            var blockingEnd = index.GetBedBlockingEnd(bed.Id, bed.RoomId, apartmentId);
            var nextApprovedStart = index.GetBedNextApprovedStart(bed.Id, bed.RoomId, apartmentId, inquiryStart);
            if (!IsUnitFreeForInquiryWindow(inquiryInstant, inquiryStart, nights, blockingEnd, nextApprovedStart))
                blockedApartmentIds.Add(apartmentId.Trim());
        }

        return blockedApartmentIds;
    }

    public async Task<HashSet<string>> GetRoomIdsWithBlockedBedsAsync(
        DateOnly inquiryStart,
        int nights,
        CancellationToken cancellationToken = default)
    {
        var inquiryInstant = AvailabilityCheckoutBlocking.ResolveInquiryStartInstant(inquiryStart);
        var index = await BuildBlockingEndIndexAsync(inquiryStart, cancellationToken);
        var roomApartmentById = await GetRoomApartmentIdsAsync(cancellationToken);
        var blockedRoomIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var beds = await dbContext.Beds
            .AsNoTracking()
            .Where(b => !b.IsDeleted)
            .Select(b => new { b.Id, b.RoomId })
            .ToListAsync(cancellationToken);

        foreach (var bed in beds)
        {
            if (string.IsNullOrWhiteSpace(bed.RoomId)) continue;
            if (!index.HasDirectBedBooking(bed.Id)) continue;
            if (!roomApartmentById.TryGetValue(bed.RoomId.Trim(), out var apartmentId)) continue;
            var blockingEnd = index.GetBedBlockingEnd(bed.Id, bed.RoomId, apartmentId);
            var nextApprovedStart = index.GetBedNextApprovedStart(bed.Id, bed.RoomId, apartmentId, inquiryStart);
            if (!IsUnitFreeForInquiryWindow(inquiryInstant, inquiryStart, nights, blockingEnd, nextApprovedStart))
                blockedRoomIds.Add(bed.RoomId.Trim());
        }

        return blockedRoomIds;
    }

    public async Task<IReadOnlyDictionary<string, IReadOnlySet<Gender>>> GetFlexibleApartmentAllowedGendersAsync(
        IEnumerable<string> apartmentIds,
        DateOnly inquiryStart,
        CancellationToken cancellationToken = default)
    {
        var ids = apartmentIds?
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray() ?? [];
        if (ids.Length == 0)
            return new Dictionary<string, IReadOnlySet<Gender>>(StringComparer.OrdinalIgnoreCase);

        var flexibleApartments = await dbContext.Apartments
            .AsNoTracking()
            .Where(a => !a.IsDeleted
                && ids.Contains(a.Id)
                && a.AllocationType == AllocationType.Flexible)
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);
        if (flexibleApartments.Count == 0)
            return new Dictionary<string, IReadOnlySet<Gender>>(StringComparer.OrdinalIgnoreCase);

        var flexibleSet = new HashSet<string>(flexibleApartments, StringComparer.OrdinalIgnoreCase);
        var inquiryInstant = AvailabilityCheckoutBlocking.ResolveInquiryStartInstant(inquiryStart);
        var blockingIndex = await BuildBlockingEndIndexAsync(inquiryStart, cancellationToken);
        var rooms = await dbContext.Rooms
            .AsNoTracking()
            .Where(r => !r.IsDeleted && flexibleSet.Contains(r.ApartmentId))
            .Select(r => new { r.Id, r.ApartmentId, r.Status })
            .ToListAsync(cancellationToken);

        var roomById = rooms.ToDictionary(r => r.Id, r => r.ApartmentId, StringComparer.OrdinalIgnoreCase);
        var beds = await dbContext.Beds
            .AsNoTracking()
            .Where(b => !b.IsDeleted && roomById.Keys.Contains(b.RoomId))
            .Select(b => new { b.Id, b.RoomId, b.Status })
            .ToListAsync(cancellationToken);

        var occupiedRoomIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var occupiedBedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var occupiedApartmentIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var room in rooms)
        {
            var blockedByStatus = room.Status is UnitStatus.Occupied or UnitStatus.Reserved;
            var blockedByInquiry =
                !IsUnitFreeOnInquiryStart(
                    inquiryInstant,
                    blockingIndex.GetRoomBlockingEnd(room.Id, room.ApartmentId));
            if (!blockedByStatus && !blockedByInquiry) continue;
            occupiedRoomIds.Add(room.Id);
            occupiedApartmentIds.Add(room.ApartmentId);
        }
        foreach (var bed in beds)
        {
            if (!roomById.TryGetValue(bed.RoomId, out var aptId)) continue;
            var blockedByStatus = bed.Status is UnitStatus.Occupied or UnitStatus.Reserved;
            var blockedByInquiry =
                !IsUnitFreeOnInquiryStart(
                    inquiryInstant,
                    blockingIndex.GetBedBlockingEnd(bed.Id, bed.RoomId, aptId));
            if (!blockedByStatus && !blockedByInquiry) continue;
            occupiedBedIds.Add(bed.Id);
            occupiedApartmentIds.Add(aptId);
        }

        var occupantGenders = await (
            from ru in dbContext.RequestUnits.AsNoTracking()
            join req in dbContext.Requests.AsNoTracking() on ru.RequestId equals req.Id
            join res in dbContext.Reservations.AsNoTracking() on req.Id equals res.RequestId
            join usr in dbContext.Users.AsNoTracking() on req.UserId equals usr.Id
            where !ru.IsDeleted
                && !req.IsDeleted
                && !res.IsDeleted
                && req.Status == Status.Approved
                && res.Status != ReservationStatus.Canceled
                && res.Status != ReservationStatus.NoShow
                && req.StartDate <= inquiryStart
                && res.EndDate > inquiryStart
                && (
                    (ru.ApartmentId != null && occupiedApartmentIds.Contains(ru.ApartmentId)) ||
                    (ru.RoomId != null && occupiedRoomIds.Contains(ru.RoomId)) ||
                    (ru.BedId != null && occupiedBedIds.Contains(ru.BedId))
                )
            select new
            {
                ru.ApartmentId,
                ru.RoomId,
                ru.BedId,
                usr.Gender
            }
        ).ToListAsync(cancellationToken);

        var result = new Dictionary<string, IReadOnlySet<Gender>>(StringComparer.OrdinalIgnoreCase);
        foreach (var aptId in flexibleSet)
        {
            var aptRooms = rooms
                .Where(r => string.Equals(r.ApartmentId, aptId, StringComparison.OrdinalIgnoreCase))
                .ToList();
            var aptRoomIds = aptRooms
                .Select(r => r.Id)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            var aptBeds = beds
                .Where(b => aptRoomIds.Contains(b.RoomId))
                .ToList();

            var hasAnyChild = aptRooms.Count > 0 || aptBeds.Count > 0;
            var allRoomsAvailable = aptRooms.All(room =>
            {
                var freeByStatus = room.Status == UnitStatus.Available;
                var freeByInquiry =
                    IsUnitFreeOnInquiryStart(
                        inquiryInstant,
                        blockingIndex.GetRoomBlockingEnd(room.Id, room.ApartmentId));
                return freeByStatus && freeByInquiry;
            });
            var allBedsAvailable = aptBeds.All(bed =>
            {
                var freeByStatus = bed.Status == UnitStatus.Available;
                var freeByInquiry =
                    IsUnitFreeOnInquiryStart(
                        inquiryInstant,
                        blockingIndex.GetBedBlockingEnd(bed.Id, bed.RoomId, aptId));
                return freeByStatus && freeByInquiry;
            });
            var allChildrenAvailable = hasAnyChild && allRoomsAvailable && allBedsAvailable;

            if (allChildrenAvailable)
            {
                result[aptId] = new HashSet<Gender> { Gender.Male, Gender.Female };
                continue;
            }

            // When some child units are occupied/reserved at inquiry time,
            // allow search genders that match the currently occupying users.
            if (!occupiedApartmentIds.Contains(aptId))
                continue;

            var genderSet = new HashSet<Gender>();
            foreach (var row in occupantGenders)
            {
                string rowApartmentId = row.ApartmentId;
                if (string.IsNullOrWhiteSpace(rowApartmentId))
                {
                    if (!string.IsNullOrWhiteSpace(row.RoomId) && roomById.TryGetValue(row.RoomId, out var fromRoom))
                        rowApartmentId = fromRoom;
                    else if (!string.IsNullOrWhiteSpace(row.BedId))
                    {
                        var bedRow = beds.FirstOrDefault(b => string.Equals(b.Id, row.BedId, StringComparison.OrdinalIgnoreCase));
                        if (bedRow != null && roomById.TryGetValue(bedRow.RoomId, out var fromBedRoom))
                            rowApartmentId = fromBedRoom;
                    }
                }

                if (!string.Equals(rowApartmentId, aptId, StringComparison.OrdinalIgnoreCase))
                    continue;
                if (row.Gender.HasValue)
                    genderSet.Add(row.Gender.Value);
            }

            if (genderSet.Count > 0)
                result[aptId] = genderSet;
        }

        return result;
    }

    private static void MergeRequestEnd(
        Dictionary<string, DateTime> requestEndById,
        string requestId,
        DateTime blockingEnd)
    {
        if (string.IsNullOrWhiteSpace(requestId)) return;
        var key = requestId.Trim();
        if (requestEndById.TryGetValue(key, out var current))
        {
            if (blockingEnd > current) requestEndById[key] = blockingEnd;
        }
        else
        {
            requestEndById[key] = blockingEnd;
        }
    }

}
