using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Bed
{
    [ExcludeFromCodeCoverage]
    public class AddBedDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string BedNumber { get; set; }
        public string Description { get; set; }
        public string Dimensions { get; set; }
        public decimal Price { get; set; }
        public UnitStatus Status { get; set; }
        public string RoomId { get; set; }
    }
}
