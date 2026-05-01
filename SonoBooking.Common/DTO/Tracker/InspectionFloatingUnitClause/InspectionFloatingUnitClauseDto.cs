using SonoTracker.Common.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.InspectionFloatingUnitClause
{
    [ExcludeFromCodeCoverage]
    public class InspectionFloatingUnitClauseDto : IEntityDto<string>
    {
        public string? Id { get; set; }
        public bool IsInspected { get; set; }
        public string? Number { get; set; }
        public string? Note { get; set; }
        public string? InspectionId { get; set; }
        public DateOnly? InspectionDate { get; set; }
        public string? InspectionClauseId { get; set; }
        public string? InspectionClauseName { get; set; }
        public string InspectionClauseCode { get; set; }
    }
}
