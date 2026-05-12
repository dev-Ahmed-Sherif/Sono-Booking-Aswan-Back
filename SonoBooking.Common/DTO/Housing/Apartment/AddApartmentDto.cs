using Microsoft.AspNetCore.Http;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Housing.UnitImage;
using SonoBooking.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Apartment
{
    [ExcludeFromCodeCoverage]
    public class AddApartmentDto : IEntityDto<string>
    {
        public string Id { get; set; }
        [MaxLength(20)]
        public string ApartmentNumber { get; set; }
        [Required, MaxLength(500)]
        public required string Description { get; set; }
        [Required]
        public required decimal Price { get; set; }
        [Required]
        public required UnitStatus Status { get; set; }
        [Required]
        public required Gender Gender { get; set; }
        [Required]
        public required AllocationType AllocationType { get; set; }
        [Required, MaxLength(50)]
        public required string Street { get; set; }
        [Required, MaxLength(20)]
        public required string BuildingNumber { get; set; }
        [Required, MaxLength(10)]
        public required string Floor { get; set; }
        [Required, MaxLength(500)]
        public required string DetailedAddress { get; set; }
        [Required, MaxLength(50)]
        public required string ApartmentTypeId { get; set; }
        [Required, MaxLength(50)]
        public required string GovernorateId { get; set; }
        [Required, MaxLength(50)]
        public required string CityId { get; set; }
        public List<AddUnitImageDto> Images { get; set; }
        public List<AddUnitImageDto> OldImages { get; set; }
    }
}
