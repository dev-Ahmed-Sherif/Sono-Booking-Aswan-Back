using SonoBooking.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.RequestParticipant
{
    [ExcludeFromCodeCoverage]
    public class AddRequestParticipantDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string RequestId { get; set; }
        public string CompanionId { get; set; }
    }
}
