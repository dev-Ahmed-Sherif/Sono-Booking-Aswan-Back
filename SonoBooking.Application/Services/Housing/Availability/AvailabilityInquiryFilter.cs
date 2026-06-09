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
    public static bool TryParseInquiryStart(string startDateHeader, out DateOnly inquiryStart) =>
        DateOnly.TryParse(startDateHeader?.Trim(), out inquiryStart);

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
        DateOnly inquiryStart,
        CancellationToken cancellationToken)
    {
        var index = await occupancyService.BuildBlockingEndIndexAsync(cancellationToken);
        var roomApartmentById = await occupancyService.GetRoomApartmentIdsAsync(cancellationToken);

        return items.Where(bed =>
        {
            string apartmentId = null;
            if (!string.IsNullOrWhiteSpace(bed.RoomId))
            {
                roomApartmentById.TryGetValue(bed.RoomId.Trim(), out apartmentId);
            }

            var blockingEnd = index.GetBedBlockingEnd(bed.Id, bed.RoomId, apartmentId);
            return occupancyService.IsUnitFreeOnInquiryStart(inquiryStart, blockingEnd);
        });
    }

    public static async Task<IEnumerable<RoomDto>> FilterRoomsAsync(
        IEnumerable<RoomDto> items,
        IUnitOccupancyService occupancyService,
        DateOnly inquiryStart,
        CancellationToken cancellationToken)
    {
        var index = await occupancyService.BuildBlockingEndIndexAsync(cancellationToken);
        return items.Where(room =>
        {
            var blockingEnd = index.GetRoomBlockingEnd(room.Id, room.ApartmentId);
            return occupancyService.IsUnitFreeOnInquiryStart(inquiryStart, blockingEnd);
        });
    }

    public static async Task<IEnumerable<ApartmentDto>> FilterApartmentsAsync(
        IEnumerable<ApartmentDto> items,
        IUnitOccupancyService occupancyService,
        DateOnly inquiryStart,
        CancellationToken cancellationToken)
    {
        var index = await occupancyService.BuildBlockingEndIndexAsync(cancellationToken);
        return items.Where(apartment =>
        {
            var blockingEnd = index.GetApartmentBlockingEnd(apartment.Id);
            return occupancyService.IsUnitFreeOnInquiryStart(inquiryStart, blockingEnd);
        });
    }

    public static async Task<IEnumerable<BedDto>> FilterBedsByGenderAsync(
        IEnumerable<BedDto> items,
        IUnitOccupancyService occupancyService,
        IReadOnlySet<Gender> genders,
        CancellationToken cancellationToken)
    {
        if (genders == null || genders.Count == 0) return items;

        var roomApartmentById = await occupancyService.GetRoomApartmentIdsAsync(cancellationToken);
        var apartmentGenders = await occupancyService.GetApartmentGendersAsync(cancellationToken);

        return items.Where(bed =>
        {
            if (string.IsNullOrWhiteSpace(bed.RoomId)) return false;
            if (!roomApartmentById.TryGetValue(bed.RoomId.Trim(), out var apartmentId)) return false;
            if (!apartmentGenders.TryGetValue(apartmentId, out var apartmentGender)) return false;
            return genders.Contains(apartmentGender);
        });
    }

    public static async Task<IEnumerable<RoomDto>> FilterRoomsByGenderAsync(
        IEnumerable<RoomDto> items,
        IUnitOccupancyService occupancyService,
        IReadOnlySet<Gender> genders,
        CancellationToken cancellationToken)
    {
        if (genders == null || genders.Count == 0) return items;

        var apartmentGenders = await occupancyService.GetApartmentGendersAsync(cancellationToken);

        return items.Where(room =>
        {
            if (string.IsNullOrWhiteSpace(room.ApartmentId)) return false;
            if (!apartmentGenders.TryGetValue(room.ApartmentId.Trim(), out var apartmentGender)) return false;
            return genders.Contains(apartmentGender);
        });
    }

    public static IEnumerable<ApartmentDto> FilterApartmentsByGender(
        IEnumerable<ApartmentDto> items,
        IReadOnlySet<Gender> genders)
    {
        if (genders == null || genders.Count == 0) return items;
        return items.Where(apartment => genders.Contains(apartment.Gender));
    }
}
