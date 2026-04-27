using SonoTracker.Common.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.Inspection
{
    [ExcludeFromCodeCoverage]
    public class InspectionDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public DateOnly InspectionDate { get; set; }
        public bool SaftyPetroleumWaste { get; set; }
        public bool RightWasteDisposal { get; set; }
        public string Note { get; set; }
        public string InspectionAttachment { get; set; }
        public string FloatingUnitId { get; set; }
        public string FloatingUnit { get; set; }
        //public string? FloatingUnitNameEn { get; set; }
        public string OrganizationId { get; set; }
        public string Organization { get; set; }
        //public string? OrganizationNameEn { get; set; }
        public string GovernorateId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}
