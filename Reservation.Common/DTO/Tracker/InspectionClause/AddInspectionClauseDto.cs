using SonoTracker.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.InspectionClause
{
    [ExcludeFromCodeCoverage]
    public class AddInspectionClauseDto : IEntityDto<string>
    {
        public string? Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string? ParentId { get; set; }
        public required string InspectionTypeId { get; set; }
    }
}
