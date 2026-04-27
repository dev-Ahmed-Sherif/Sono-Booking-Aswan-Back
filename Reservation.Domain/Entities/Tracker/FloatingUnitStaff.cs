using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using SonoTracker.Domain.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class FloatingUnitStaff : BaseEntity<string>
    {
        public FloatingUnitStaff()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [Required, MaxLength(100)]
        public required string Name { get; set; }

        [Required, MaxLength(100)]
        public required string Job { get; set; }

        [Required, MaxLength(100)]
        public required string Mobile { get; set; }

        [Required, MaxLength(100)]
        public required string Email { get; set; }
        
        public Gender Gender { get; set; }
        public IDType IDType { get; set; }
        
        [Required, MaxLength(20)]
        public required string Identity { get; set; }

        public bool IsDelegate { get; set; }
        public string? DelegateAttachment { get; set; }

        [Required, MaxLength(50)] 
        [ForeignKey(nameof(Nationality))]
        public required string NationalityId { get; set; }
        public virtual Nationality? Nationality { get; set; }
        [Required, MaxLength(50)]
        [ForeignKey(nameof(FloatingUnit))]
        public required string FloatingUnitId { get; set; }
        public virtual FloatingUnit? FloatingUnit { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }
    }
}
