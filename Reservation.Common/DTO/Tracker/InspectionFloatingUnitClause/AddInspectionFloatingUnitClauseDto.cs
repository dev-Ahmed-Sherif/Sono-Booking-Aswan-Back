using SonoTracker.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.InspectionFloatingUnitClause
{
    [ExcludeFromCodeCoverage]
    public class AddInspectionFloatingUnitClauseDto : IEntityDto<string>
    {
        public string? Id { get; set; }
        public required bool IsInspected { get; set; }
        public string? Number { get; set; }
        public string? Note { get; set; }
        public string InspectionId { get; set; }
        public required string InspectionClauseId { get; set; }
    }
}
