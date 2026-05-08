using SonoBooking.Common.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.RequestParticipant
{
    [ExcludeFromCodeCoverage]
    public class EditRequestParticipantDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string RequestId { get; set; }
        public string CompanionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
