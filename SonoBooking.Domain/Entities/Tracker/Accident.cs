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
   public class Accident : BaseAudit<string>
   {
        public Accident()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [Required]
        public required string Code { get; set; }
        public DateOnly AccidentDate { get; set; }
        public DateOnly? ResponseDate { get; set; }
        public Case Case { get; set; }
        public string? Notes { get; set; }
        public string? Attach { get; set; }

      
        [MaxLength(50), ForeignKey(nameof(City))]
        public string? CityId { get; set; }
        public virtual City? City { get; set; }

        [MaxLength(50), ForeignKey(nameof(GeoPoint))]
        public string? GeoPointId { get; set; }
        public virtual GeoPoint? GeoPoint { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(AccidentType))]
        public required string AccidentTypeId { get; set; }
        public virtual AccidentType? AccidentType { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(FloatingUnit))]
        public required string FloatingUnitId { get; set; }
        public virtual FloatingUnit? FloatingUnit { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        public virtual HashSet<AccidentOrganization> AccidentOrganizations { get; set; } = [];
        public virtual HashSet<AccidentAttachment> AccidentAttachments { get; set; } = [];
    }
}
