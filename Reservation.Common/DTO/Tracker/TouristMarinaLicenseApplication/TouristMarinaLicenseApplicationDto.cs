using SonoTracker.Common.Core;
using SonoTracker.Domain.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.TouristMarinaLicenseApplication
{
    [ExcludeFromCodeCoverage]

    public class TouristMarinaLicenseApplicationDto : IEntityDto<string>
    {
        public string? Id { get; set; }
        public string LicenseNumber { get; set; }
        public DateOnly LicenseDate { get; set; }
        public string FromOrganizationId { get; set; }
        public string FromOrganizationNameAr { get; set; }
        public string ToOrganizationId { get; set; }
        public string ToOrganizationNameAr { get; set; }
        public string Text { get; set; }
        public string Status { get; set; }
        public string Insurance { get; set; }
        public string CommercialRegister { get; set; }
        public string Taxes { get; set; }
        public string CivilProtection { get; set; }
        public string Irrigation { get; set; }
        public string StateProperty { get; set; }
        public string Other { get; set; }
        public int TouristMarinaNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}
