using SonoBooking.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.RequestUnit
{
    [ExcludeFromCodeCoverage]
    public class AddRequestUnitDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string RequestId { get; set; }
        public string ApartmentId { get; set; }
        public string BedId { get; set; }
        public string RoomId { get; set; }
    }
}
