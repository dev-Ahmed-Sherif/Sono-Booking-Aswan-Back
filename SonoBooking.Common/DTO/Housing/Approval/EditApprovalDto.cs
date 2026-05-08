using SonoBooking.Common.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Approval
{
    [ExcludeFromCodeCoverage]
    public class EditApprovalDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string RequestId { get; set; }
        public string LeaderId { get; set; }
        public string Decision { get; set; }
        public string Notes { get; set; }
        public DateTime DecisionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
