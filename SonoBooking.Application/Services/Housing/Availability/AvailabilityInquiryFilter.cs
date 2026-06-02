using SonoBooking.Common.DTO.Housing.Apartment;
using SonoBooking.Common.DTO.Housing.Bed;
using SonoBooking.Common.DTO.Housing.Room;
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
}
