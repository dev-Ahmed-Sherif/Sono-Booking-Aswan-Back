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
    public class TouristMarinaLicenseApplication : BaseAudit<string>
    {
        public TouristMarinaLicenseApplication()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [MaxLength(100)]
        public required string LicenseNumber { get; set; }
        public required DateOnly LicenseDate { get; set; }
        public string? LicenseNote { get; set; }
        public Status Status { get; set; } = Status.Pending;
        public required string Insurance { get; set; }
        public required string CommercialRegister { get; set; }
        public required string Taxes { get; set; }
        public required string CivilProtection { get; set; }
        public required string Irrigation { get; set; }
        public required string StateProperty { get; set; }
        public string? OtherAttach { get; set; }
        public int TouristMarinaNumber { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(FromOrganization))]
        public required string FromOrganizationId { get; set; }
        public virtual Organization? FromOrganization { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(ToOrganization))]
        public required string ToOrganizationId { get; set; }
        public virtual Organization? ToOrganization { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }
    }
}
