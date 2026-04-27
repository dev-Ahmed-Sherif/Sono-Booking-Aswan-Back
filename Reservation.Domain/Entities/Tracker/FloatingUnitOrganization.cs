using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class FloatingUnitOrganization : BaseEntity<string>
    {
        public FloatingUnitOrganization()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
        
        [Required, MaxLength(50)]
        [ForeignKey(nameof(Organization))]
        public required string OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(FloatingUnit))]
        public required string FloatingUnitId { get; set; }
        public virtual FloatingUnit? FloatingUnit { get; set; }
    }
}
