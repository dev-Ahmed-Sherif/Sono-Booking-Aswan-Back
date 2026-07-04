using SonoBooking.Common.DTO.Housing.Apartment;
using SonoBooking.Common.DTO.Housing.Bed;
using SonoBooking.Common.DTO.Housing.Room;
using SonoBooking.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Availability;

public static class AvailabilityInquiryFilter
{
    public static bool TryParseInquiryStart(string startDateHeader, out DateOnly inquiryStart)
    {
        if (AvailabilityCheckoutBlocking.TryParseInquiryStartInstant(startDateHeader, out var instant))
        {
            inquiryStart = DateOnly.FromDateTime(instant.Date);
            return true;
        }

        inquiryStart = default;
        return false;
    }

    public static bool TryParseInquiryStartInstant(string startDateHeader, out DateTime inquiryStart) =>
        AvailabilityCheckoutBlocking.TryParseInquiryStartInstant(startDateHeader, out inquiryStart);

    /// <summary>
    /// Parses `Gender` header: comma/semicolon-separated tokens (Male, Female, male, female, 1, 2, Arabic labels).
    /// </summary>
    public static bool TryParseGenders(string genderHeader, out HashSet<Gender> genders)
    {
        genders = new HashSet<Gender>();
        if (string.IsNullOrWhiteSpace(genderHeader)) return false;

        foreach (var part in genderHeader.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries))
        {
            if (TryParseGenderToken(part, out var gender))
            {
                genders.Add(gender);
            }
        }

        return genders.Count > 0;
    }

    private static bool TryParseGenderToken(string token, out Gender gender)
    {
        gender = default;
        var text = token.Trim();
        if (text.Length == 0) return false;

        if (Enum.TryParse(text, ignoreCase: true, out gender))
        {
            return Enum.IsDefined(typeof(Gender), gender);
        }

        var lower = text.ToLowerInvariant();
        if (lower is "1" or "m" or "male" or "man" or "ذكر" or "رجال")
        {
            gender = Gender.Male;
            return true;
        }

        if (lower is "2" or "f" or "female" or "woman" or "أنثى" or "انثى" or "سيدات")
        {
            gender = Gender.Female;
            return true;
        }

        return false;
    }

    public static async Task<IEnumerable<BedDto>> FilterBedsAsync(
        IEnumerable<BedDto> items,
        IUnitOccupancyService occupancyService,
        DateTime inquiryStartInstant,
        DateOnly inquiryStartDate,
        int? nights,
        CancellationToken cancellationToken)
    {
        var index = await occupancyService.BuildBlockingEndIndexAsync(inquiryStartDate, cancellationToken);
        var roomApartmentById = await occupancyService.GetRoomApartmentIdsAsync(cancellationToken);

        return items.Where(bed =>
        {
            string apartmentId = null;
            if (!string.IsNullOrWhiteSpace(bed.RoomId))
            {
                roomApartmentById.TryGetValue(bed.RoomId.Trim(), out apartmentId);
            }

            var blockingEnd = index.GetBedBlockingEnd(bed.Id, bed.RoomId, apartmentId);
            var nextApprovedStart = index.GetBedNextApprovedStart(bed.Id, bed.RoomId, apartmentId, inquiryStartDate);
            return occupancyService.IsUnitFreeForInquiryWindow(
                inquiryStartInstant,
                inquiryStartDate,
                nights ?? 0,
                blockingEnd,
                nextApprovedStart);
        });
    }

    public static async Task<IEnumerable<RoomDto>> FilterRoomsAsync(
        IEnumerable<RoomDto> items,
        IUnitOccupancyService occupancyService,
        DateTime inquiryStartInstant,
        DateOnly inquiryStartDate,
        int? nights,
        CancellationToken cancellationToken)
    {
        var index = await occupancyService.BuildBlockingEndIndexAsync(inquiryStartDate, cancellationToken);
        var nightsValue = nights ?? 0;
        var roomsWithBlockedBeds = await occupancyService.GetRoomIdsWithBlockedBedsAsync(
            inquiryStartDate,
            nightsValue,
            cancellationToken);

        return items.Where(room =>
        {
            if (!string.IsNullOrWhiteSpace(room.Id) &&
                roomsWithBlockedBeds.Contains(room.Id.Trim()))
                return false;

            var blockingEnd = index.GetRoomBlockingEnd(room.Id, room.ApartmentId);
            var nextApprovedStart = index.GetRoomNextApprovedStart(room.Id, room.ApartmentId, inquiryStartDate);
            return occupancyService.IsUnitFreeForInquiryWindow(
                inquiryStartInstant,
                inquiryStartDate,
                nightsValue,
                blockingEnd,
                nextApprovedStart);
        });
    }

    public static async Task<IEnumerable<ApartmentDto>> FilterApartmentsAsync(
        IEnumerable<ApartmentDto> items,
        IUnitOccupancyService occupancyService,
        DateTime inquiryStartInstant,
        DateOnly inquiryStartDate,
        int? nights,
        CancellationToken cancellationToken)
    {
        var index = await occupancyService.BuildBlockingEndIndexAsync(inquiryStartDate, cancellationToken);
        var nightsValue = nights ?? 0;
        var apartmentsWithAvailableChildren =
            await occupancyService.GetApartmentIdsWithAvailableChildrenAsync(
                inquiryStartDate,
                nightsValue,
                cancellationToken);
        var apartmentsWithBlockedChildren =
            await occupancyService.GetApartmentIdsWithBlockedChildrenAsync(
                inquiryStartDate,
                nightsValue,
                cancellationToken);

        return items.Where(apartment =>
        {
            var blockingEnd = index.GetApartmentBlockingEnd(apartment.Id);
            var nextApprovedStart = index.GetApartmentNextApprovedStart(apartment.Id, inquiryStartDate);
            if (!occupancyService.IsUnitFreeForInquiryWindow(
                    inquiryStartInstant,
                    inquiryStartDate,
                    nightsValue,
                    blockingEnd,
                    nextApprovedStart))
                return false;

            if (!string.IsNullOrWhiteSpace(apartment.Id) &&
                apartmentsWithBlockedChildren.Contains(apartment.Id.Trim()))
                return false;

            // Whole-apartment bookings use apartment-level index entries.
            // Otherwise require at least one free child room/bed (flexible partial occupancy).
            return index.HasDirectApartmentBooking(apartment.Id) ||
                   apartmentsWithAvailableChildren.Contains(apartment.Id);
        });
    }

    public static async Task<IEnumerable<BedDto>> FilterBedsByGenderAsync(
        IEnumerable<BedDto> items,
        IUnitOccupancyService occupancyService,
        IReadOnlySet<Gender> genders,
        DateOnly? inquiryStart,
        CancellationToken cancellationToken)
    {
        if (genders == null || genders.Count == 0) return items;

        var roomApartmentById = await occupancyService.GetRoomApartmentIdsAsync(cancellationToken);
        var apartmentGenders = await occupancyService.GetApartmentGendersAsync(cancellationToken);
        var apartmentIds = items
            .Where(b => !string.IsNullOrWhiteSpace(b.RoomId))
            .Select(b => b.RoomId.Trim())
            .Where(roomApartmentById.ContainsKey)
            .Select(roomId => roomApartmentById[roomId])
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var flexibleOverrides = await occupancyService.GetFlexibleApartmentAllowedGendersAsync(
            apartmentIds,
            inquiryStart ?? DateOnly.MinValue,
            cancellationToken);

        return items.Where(bed =>
        {
            if (string.IsNullOrWhiteSpace(bed.RoomId)) return false;
            if (!roomApartmentById.TryGetValue(bed.RoomId.Trim(), out var apartmentId)) return false;
            if (!apartmentGenders.TryGetValue(apartmentId, out var apartmentGender) &&
                !flexibleOverrides.TryGetValue(apartmentId, out _)) return false;
            if (apartmentGenders.TryGetValue(apartmentId, out apartmentGender) &&
                genders.Contains(apartmentGender)) return true;
            return flexibleOverrides.TryGetValue(apartmentId, out var extra) &&
                extra.Any(genders.Contains);
        });
    }

    public static async Task<IEnumerable<RoomDto>> FilterRoomsByGenderAsync(
        IEnumerable<RoomDto> items,
        IUnitOccupancyService occupancyService,
        IReadOnlySet<Gender> genders,
        DateOnly? inquiryStart,
        CancellationToken cancellationToken)
    {
        if (genders == null || genders.Count == 0) return items;

        var apartmentGenders = await occupancyService.GetApartmentGendersAsync(cancellationToken);
        var apartmentIds = items
            .Where(r => !string.IsNullOrWhiteSpace(r.ApartmentId))
            .Select(r => r.ApartmentId.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var flexibleOverrides = await occupancyService.GetFlexibleApartmentAllowedGendersAsync(
            apartmentIds,
            inquiryStart ?? DateOnly.MinValue,
            cancellationToken);

        return items.Where(room =>
        {
            if (string.IsNullOrWhiteSpace(room.ApartmentId)) return false;
            var apartmentId = room.ApartmentId.Trim();
            if (!apartmentGenders.TryGetValue(apartmentId, out var apartmentGender) &&
                !flexibleOverrides.TryGetValue(apartmentId, out _)) return false;
            if (apartmentGenders.TryGetValue(apartmentId, out apartmentGender) &&
                genders.Contains(apartmentGender)) return true;
            return flexibleOverrides.TryGetValue(apartmentId, out var extra) &&
                extra.Any(genders.Contains);
        });
    }

    public static IEnumerable<ApartmentDto> FilterApartmentsByGender(
        IEnumerable<ApartmentDto> items,
        IReadOnlySet<Gender> genders,
        IReadOnlyDictionary<string, IReadOnlySet<Gender>> flexibleOverrides)
    {
        if (genders == null || genders.Count == 0) return items;
        return items.Where(apartment =>
            genders.Contains(apartment.Gender) ||
            (flexibleOverrides.TryGetValue(apartment.Id, out var extra) &&
             extra.Any(genders.Contains)));
    }
}
