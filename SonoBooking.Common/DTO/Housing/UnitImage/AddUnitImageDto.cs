using Microsoft.AspNetCore.Http;
using SonoBooking.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.UnitImage
{
    [ExcludeFromCodeCoverage]
    public class AddUnitImageDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string ApartmentId { get; set; }
        public string RoomId { get; set; }
        public string BedId { get; set; }
        public required string AttachmentId { get; set; }
        public bool IsPrimary { get; set; }
        public required IFormFile Image { get; set; }
    }
}
