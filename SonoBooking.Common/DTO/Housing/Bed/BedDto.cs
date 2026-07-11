using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Housing.UnitImage;
using SonoBooking.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Bed
{
    [ExcludeFromCodeCoverage]
    public class BedDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string BedNumber { get; set; }
        public string Description { get; set; }
        public string Dimensions { get; set; }
        public decimal Price { get; set; }
        public UnitStatus Status { get; set; }
        public bool AdministrativeStatus { get; set; }
        public DateOnly? StartAdministrativeDate { get; set; }
        public DateOnly? EndAdministrativeDate { get; set; }
        public string RoomId { get; set; }
        public virtual ICollection<UnitImageDto> Images { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
