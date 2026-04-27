using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.InspectionFloatingUnitClause.Parameters
{
    [ExcludeFromCodeCoverage]
    public class InspectionFloatingUnitClauseFilter
    {
        public string? InspectionId { get; set; }
        public string? InspectionClauseId { get; set; }
        public bool? IsInspected { get; set; }
    }
}
