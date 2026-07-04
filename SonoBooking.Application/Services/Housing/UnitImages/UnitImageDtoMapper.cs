using SonoBooking.Common.DTO.Housing.UnitImage;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Linq;

namespace SonoBooking.Application.Services.Housing.UnitImages;

internal static class UnitImageDtoMapper
{
    internal static List<UnitImageDto> MapFromEntities(IEnumerable<UnitImage>? unitImages)
    {
        if (unitImages == null)
            return [];

        return unitImages
            .Where(ui => ui is { IsDeleted: false, Attachment: not null })
            .Select(ui => new UnitImageDto
            {
                Id = ui.Id,
                AttachmentId = ui.AttachmentId,
                ApartmentId = ui.ApartmentId,
                RoomId = ui.RoomId,
                BedId = ui.BedId,
                Name = ui.Attachment!.FileName,
                Url = ui.Attachment.Url,
                IsPrimary = ui.IsPrimary,
            })
            .ToList();
    }
}
