using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class TouristMarinaOrganization : BaseEntity<string>
    {
        public TouristMarinaOrganization()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [Required, MaxLength(50)]
        public required string LicenseNumber { get; set; }
        public required DateOnly FromDate { get; set; }
        public required DateOnly ToDate { get; set; }
        public bool IsActive { get; set; }
        
        [Required, MaxLength(50)]
        [ForeignKey(nameof(TouristMarina))]
        public required string TouristMarinaId { get; set; }
        public virtual TouristMarina? TouristMarina { get; set; }
        
        [Required, MaxLength(50)]
        [ForeignKey(nameof(Organization))]
        public required string OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }
    }
}
