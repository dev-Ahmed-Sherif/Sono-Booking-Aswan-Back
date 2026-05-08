using SonoBooking.Common.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Approval
{
    [ExcludeFromCodeCoverage]
    public class AddApprovalDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string RequestId { get; set; }
        public string LeaderId { get; set; }
        public string Decision { get; set; }
        public string Notes { get; set; }
        public DateTime DecisionDate { get; set; }
    }
}
