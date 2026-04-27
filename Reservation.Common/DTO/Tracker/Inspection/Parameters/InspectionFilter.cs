using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.Inspection.Parameters
{
    [ExcludeFromCodeCoverage]
    public class InspectionFilter
    {
        public DateTime? InspectionDate { get; set; }
        public string? FloatingUnitId { get; set; }
        public string? OrganizationId { get; set; }
        public string? GovernorateId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
