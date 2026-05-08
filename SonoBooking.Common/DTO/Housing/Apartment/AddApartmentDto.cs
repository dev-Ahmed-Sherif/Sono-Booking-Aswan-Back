using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Apartment
{
    [ExcludeFromCodeCoverage]
    public class AddApartmentDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string ApartmentNumber { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public UnitStatus Status { get; set; }
        public Gender Gender { get; set; }
        public AllocationType AllocationType { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
        public string Floor { get; set; }
        public string DetailedAddress { get; set; }
        public string ApartmentTypeId { get; set; }
        public string GovernorateId { get; set; }
        public string CityId { get; set; }
    }
}
