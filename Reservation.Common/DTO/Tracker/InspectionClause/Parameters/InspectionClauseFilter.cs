using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.InspectionClause.Parameters
{
    [ExcludeFromCodeCoverage]
    public class InspectionClauseFilter
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ParentId { get; set; }
        public string? InspectionTypeId { get; set; }
    }
}
