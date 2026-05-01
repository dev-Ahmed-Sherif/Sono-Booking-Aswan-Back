using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using SonoTracker.Domain.Enum;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class InspectionFloatingUnitClause : BaseEntity<string>
    {
        public InspectionFloatingUnitClause()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
        public required bool IsInspected { get; set; }
        
        [MaxLength(4)]
        public string? Number { get; set; }
        
        [MaxLength(50)]
        public string? Note { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(Inspection))]
        public required string InspectionId { get; set; }
        public virtual Inspection? Inspection { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(InspectionClause))]
        public required string InspectionClauseId { get; set; }
        public virtual InspectionClause? InspectionClause { get; set; }
    }
}
