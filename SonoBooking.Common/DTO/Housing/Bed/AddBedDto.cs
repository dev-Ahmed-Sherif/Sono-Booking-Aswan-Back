using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Housing.UnitImage;
using SonoBooking.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Bed
{
    [ExcludeFromCodeCoverage]
    public class AddBedDto : IEntityDto<string>
    {
        public string Id { get; set; }
        [Required, MaxLength(20)]
        public required string BedNumber { get; set; }
        [Required, MaxLength(500)]
        public required string Description { get; set; }
        [Required, MaxLength(100)]
        public required string Dimensions { get; set; }
        [Required]
        public required decimal Price { get; set; }
        [Required]
        public required UnitStatus Status { get; set; }
        [Required, MaxLength(50)]
        public required string RoomId { get; set; }
        public List<AddUnitImageDto> Images { get; set; }
        public List<AddUnitImageDto> OldImages { get; set; }
    }
}
