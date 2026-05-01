using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using SonoTracker.Domain.Enum;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class Inspection : BaseAudit<string>
    {
        public Inspection()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        public string Number { get; set; }
        public DateOnly InspectionDate { get; set; }
        public bool SaftyPetroleumWaste { get; set; }
        public bool RightWasteDisposal { get; set; }
        public string? Note { get; set; }
        public string? InspectionAttachment { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(InspectionType))]
        public required string InspectionTypeId { get; set; }
        public virtual InspectionType? InspectionType { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(FloatingUnit))]
        public required string FloatingUnitId { get; set; }
        public virtual FloatingUnit? FloatingUnit { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(Organization))]
        public required string OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        public virtual HashSet<InspectionFloatingUnitClause> InspectionFloatingUnitClauses { get; set; } = [];
        public virtual HashSet<InspectionAttachment> InspectionAttachments { get; set; } = [];
    }
}
