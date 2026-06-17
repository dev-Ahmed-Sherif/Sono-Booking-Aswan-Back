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

    public bool IsUnitFreeForInquiryWindow(
        DateOnly inquiryStart,
        int nights,
        DateOnly? blockingEnd,
        DateOnly? nextApprovedStart)
    {
        if (!IsUnitFreeOnInquiryStart(inquiryStart, blockingEnd))
            return false;

        if (!nextApprovedStart.HasValue)
            return true;

        // Enhanced rule: hide when an approved stay is still active at inquiry start (noon rule).
        if (nextApprovedStart.Value <= inquiryStart &&
            !IsUnitFreeOnInquiryStart(inquiryStart, blockingEnd))
            return false;

        if (nights <= 0)
            return true;

        // Past occupancy ended before this inquiry — no overlap check needed.
        if (blockingEnd.HasValue && blockingEnd.Value < inquiryStart)
            return true;

        var inquiryEnd = inquiryStart.AddDays(nights);

        // Nights overlap applies only to a future approved stay on this unit.
        if (nextApprovedStart.Value <= inquiryStart)
            return true;

        return inquiryEnd < nextApprovedStart.Value;
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
                && r.Status != ReservationStatus.NoShow)
            .Select(r => new { r.Id, r.RequestId, r.ActualCheckOutDate, r.EndDate })
            .ToListAsync(cancellationToken);

        var requestIdByReservationId = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var requestEndById = new Dictionary<string, DateOnly>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in reservationRows)
        {
            if (string.IsNullOrWhiteSpace(row.RequestId)) continue;
            var endYmd = row.ActualCheckOutDate is not null
                ? DateOnly.FromDateTime(row.ActualCheckOutDate.Value.Date)
                : row.EndDate;
            var key = row.RequestId.Trim();
            MergeRequestEnd(requestEndById, key, endYmd);
            if (!string.IsNullOrWhiteSpace(row.Id))
                requestIdByReservationId[row.Id.Trim()] = key;
        }

        var approvedExtensionEntities = await dbContext.Extensions
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.Status == Status.Approved)
            .Select(e => new { e.ReservationId, e.EndDate })
            .ToListAsync(cancellationToken);

        foreach (var ext in approvedExtensionEntities)
        {
            if (string.IsNullOrWhiteSpace(ext.ReservationId)) continue;
            if (!requestIdByReservationId.TryGetValue(ext.ReservationId.Trim(), out var requestId)) continue;
            MergeRequestEnd(requestEndById, requestId, ext.EndDate);
        }

        var previousRequestById = await dbContext.Requests
            .AsNoTracking()
            .Where(r => !r.IsDeleted)
            .Select(r => new { r.Id, r.PreviousRequestId })
            .ToDictionaryAsync(
                r => r.Id,
                r => r.PreviousRequestId,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        var approvedRequests = await dbContext.Requests
            .AsNoTracking()
            .Where(r => !r.IsDeleted && r.Status == Status.Approved)
            .Select(r => new { r.Id, r.StartDate, r.EndDate, r.PreviousRequestId, r.RequestCatagory })
            .ToListAsync(cancellationToken);

        foreach (var req in approvedRequests)
        {
            MergeRequestEnd(requestEndById, req.Id, req.EndDate);
            if (req.RequestCatagory == RequestCatagory.Extension
                && !string.IsNullOrWhiteSpace(req.PreviousRequestId))
            {
                var rootId = ResolveRootStayRequestId(req.PreviousRequestId, previousRequestById);
                MergeRequestEnd(requestEndById, rootId, req.EndDate);
            }
        }

        var requestNextStartById = approvedRequests
            .ToDictionary(r => r.Id, r => r.StartDate, StringComparer.OrdinalIgnoreCase);

        var requestUnits = await dbContext.RequestUnits
            .AsNoTracking()
            .Include(ru => ru.Bed)
            .ThenInclude(b => b.Room)
            .ToListAsync(cancellationToken);

        foreach (var unit in requestUnits)
        {
            if (string.IsNullOrWhiteSpace(unit.RequestId)) continue;
            if (!requestEndById.TryGetValue(unit.RequestId, out var endYmd)) continue;
            if (!requestNextStartById.TryGetValue(unit.RequestId, out var requestStart)) continue;

            // Ignore completed stays for future inquiries.
            if (inquiryStart.HasValue && endYmd < inquiryStart.Value)
                continue;

            // Apply blocking only on the most specific booked unit so sibling beds/rooms
            // can remain available (flexible partial occupancy).
            if (!string.IsNullOrWhiteSpace(unit.BedId))
            {
                UnitBlockingEndIndex.SetMax(index.Beds, unit.BedId, endYmd);
                UnitBlockingEndIndex.SetMin(index.BedNextApprovedStarts, unit.BedId, requestStart);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(unit.RoomId))
            {
                UnitBlockingEndIndex.SetMax(index.Rooms, unit.RoomId, endYmd);
                UnitBlockingEndIndex.SetMin(index.RoomNextApprovedStarts, unit.RoomId, requestStart);
                continue;
            }

            if (!string.IsNullOrWhiteSpace(unit.ApartmentId))
            {
                UnitBlockingEndIndex.SetMax(index.Apartments, unit.ApartmentId, endYmd);
                UnitBlockingEndIndex.SetMin(index.ApartmentNextApprovedStarts, unit.ApartmentId, requestStart);
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
            var nextApprovedStart = index.GetRoomNextApprovedStart(room.Id, room.ApartmentId);
            if (IsUnitFreeForInquiryWindow(inquiryStart, nights, blockingEnd, nextApprovedStart))
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
            var nextApprovedStart = index.GetBedNextApprovedStart(bed.Id, bed.RoomId, apartmentId);
            if (IsUnitFreeForInquiryWindow(inquiryStart, nights, blockingEnd, nextApprovedStart))
                availableApartmentIds.Add(apartmentId.Trim());
        }

        return availableApartmentIds;
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
                    inquiryStart,
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
                    inquiryStart,
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
                && (res.ActualCheckOutDate == null || DateOnly.FromDateTime(res.ActualCheckOutDate.Value.Date) >= inquiryStart)
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
                        inquiryStart,
                        blockingIndex.GetRoomBlockingEnd(room.Id, room.ApartmentId));
                return freeByStatus && freeByInquiry;
            });
            var allBedsAvailable = aptBeds.All(bed =>
            {
                var freeByStatus = bed.Status == UnitStatus.Available;
                var freeByInquiry =
                    IsUnitFreeOnInquiryStart(
                        inquiryStart,
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
        Dictionary<string, DateOnly> requestEndById,
        string requestId,
        DateOnly endYmd)
    {
        if (string.IsNullOrWhiteSpace(requestId)) return;
        var key = requestId.Trim();
        if (requestEndById.TryGetValue(key, out var current))
        {
            if (endYmd > current) requestEndById[key] = endYmd;
        }
        else
        {
            requestEndById[key] = endYmd;
        }
    }

    private static string ResolveRootStayRequestId(
        string requestId,
        IReadOnlyDictionary<string, string?> previousRequestById)
    {
        var current = requestId.Trim();
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (previousRequestById.TryGetValue(current, out var previous)
            && !string.IsNullOrWhiteSpace(previous))
        {
            if (!visited.Add(current)) break;
            current = previous.Trim();
        }

        return current;
    }
}
