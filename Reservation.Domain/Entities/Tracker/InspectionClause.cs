using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using SonoTracker.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class InspectionClause : BaseAudit<string>
    {
        public InspectionClause()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        public required string Code { get; set; }
        public required string Name { get; set; }

        [MaxLength(50)]
        [ForeignKey(nameof(Parent))]
        public string? ParentId { get; set; }
        public virtual InspectionClause? Parent { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(InspectionType))]
        public required string InspectionTypeId { get; set; }
        public virtual InspectionType? InspectionType { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }
        
        public virtual HashSet<InspectionFloatingUnitClause> InspectionFloatingUnitClauses { get; set; } = [];
    }
}
