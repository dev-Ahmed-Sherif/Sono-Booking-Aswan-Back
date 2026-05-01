using System;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections;
using System.Collections.Generic;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class Maintenance : BaseAudit<string>
    {
        public Maintenance()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        public required string Number { get; set; }
        public required DateOnly MaintenanceDate { get; set; }
        public DateOnly? NextMaintenanceDate { get; set; }
        public required string MaintenanceReport { get; set; }
        public string Notes { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(MaintenanceType))]
        public required string MaintenanceTypeId { get; set; }
        public virtual MaintenanceType? MaintenanceType { get; set; }

        [Required, MaxLength(50)] 
        [ForeignKey(nameof(FloatingUnit))]
        public required string FloatingUnitId { get; set; }
        public virtual FloatingUnit? FloatingUnit { get; set; }

        [MaxLength(50)]
        [ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        public HashSet<MaintenanceAttachment> MaintenanceAttachments { get; set; } = [];
    }
}
