using SonoTracker.Domain.Enum;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.TouristMarinaLicenseApplication.Parameters
{
    [ExcludeFromCodeCoverage]

    public class TouristMarinaLicenseApplicationFilter
    {
        public string LicenseNumber { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public string? FromOrganizationId { get; set; }
        public string? ToOrganizationId { get; set; }
        public Status? Status { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
