using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class OrganizationStaff : BaseEntity<string>
    {
        public OrganizationStaff()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Job { get; set; }
        [MaxLength(100)]
        public string? Phone { get; set; }
        [MinLength(100)]
        public string Mobile { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(20)]
        public string? PhoneExtension { get; set; }

        [Required]
        [MaxLength(14)]
        public required string NationalId { get; set; }
        public bool IsDelegate { get; set; }
        public string? DelegateAttachment { get; set; }

        [MaxLength(50), ForeignKey(nameof(Organization))]
        public string? OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }
    }
}