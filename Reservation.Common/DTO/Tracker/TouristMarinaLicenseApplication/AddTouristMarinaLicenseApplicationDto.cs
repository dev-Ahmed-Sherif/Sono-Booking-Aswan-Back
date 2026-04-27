using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;
using SonoTracker.Domain.Enum;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.TouristMarinaLicenseApplication
{
    [ExcludeFromCodeCoverage]
    public class AddTouristMarinaLicenseApplicationDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string LicenseNumber { get; set; }
        public Status Status { get; set; }
        public DateOnly LicenseDate { get; set; }
        public string LicenseNote { get; set; }
        public required IFormFile Insurance { get; set; }
        public required IFormFile CommercialRegister { get; set; }
        public required IFormFile Taxes { get; set; }
        public required IFormFile CivilProtection { get; set; }
        public required IFormFile Irrigation { get; set; }
        public required IFormFile StateProperty { get; set; }
        public IFormFile Other { get; set; }
        public required string FromOrganizationId { get; set; }
        public required string ToOrganizationId { get; set; }
        public int TouristMarinaNumber { get; set; }
    }
}
