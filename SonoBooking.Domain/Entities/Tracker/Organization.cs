using SonoTracker.Domain.Entities.Attachments;
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
    public class Organization : Lookup<string>
    {
        public Organization()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
        [MaxLength(2)]
        public string? TouristMarinaNumber { get; set; }

        [Required, MaxLength(200)]
        public required string Address { get; set; }

        [MaxLength(100)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Fax { get; set; }

        [MaxLength(100)]
        public string? Mobile { get; set; }

        [Required, MaxLength(100)]
        public required string Email { get; set; }
        public OrganizationType OrganizationType { get; set; }
        public string? CommercialRegistrationAttachment { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsReport { get; set; }

        [MaxLength(50)]
        public string? CommercialRegistrationNumber { get; set; }
        public string? WebSiteAddress { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        [MaxLength(50), ForeignKey(nameof(Nationality))]
        public string? NationalityId { get; set; }
        public virtual Nationality? Nationality { get; set; }

        [MaxLength(50), ForeignKey(nameof(OrganizationCategory))]
        public string? OrganizationCategoryId { get; set; }
        public virtual OrganizationCategory? OrganizationCategory { get; set; }

        //[MaxLength(50), ForeignKey(nameof(InspectionType))]
        //public string? InspectionTypeId { get; set; }
        //public virtual InspectionType? InspectionType { get; set; }
        public virtual HashSet<OrganizationStaff> OrganizationStaffs { get; set; } = [];
        public virtual HashSet<TouristMarinaOrganization> TouristMarinaOrganizations { get; set; } = [];
        public virtual HashSet<FloatingUnitOrganization> FloatingUnitOrganizations { get; set; } = [];
        public virtual HashSet<Inspection> Inspections { get; set; } = [];
        public virtual HashSet<TouristMarinaLicenseApplication> TouristMarinaLicenseApplication { get; set; } = [];
        public virtual HashSet<Accident> Accidents { get; set; } = [];
        public virtual HashSet<Maintenance> Maintenances { get; set; } = [];
    }
}
