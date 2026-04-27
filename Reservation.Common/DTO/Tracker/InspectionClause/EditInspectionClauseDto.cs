using SonoTracker.Common.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.InspectionClause
{
    [ExcludeFromCodeCoverage]
    public class EditInspectionClauseDto : IEntityDto<string>
    {
        public string? Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ParentId { get; set; }
        public string? ParentName { get; set; }
        public string? InspectionTypeId { get; set; }
        public string? InspectionTypeName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public string? ModifiedById { get; set; }
    }
}
