using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Room
{
    [ExcludeFromCodeCoverage]
    public class AddRoomDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string RoomNumber { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public UnitStatus Status { get; set; }
        public string ApartmentId { get; set; }
        public string RoomTypeId { get; set; }
    }
}
