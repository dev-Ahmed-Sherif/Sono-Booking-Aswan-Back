using SonoBooking.Common.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.UnitImage
{
    [ExcludeFromCodeCoverage]
    public class EditUnitImageDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string? ApartmentId { get; set; }
        public string? RoomId { get; set; }
        public string? BedId { get; set; }
        public string AttachmentId { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
