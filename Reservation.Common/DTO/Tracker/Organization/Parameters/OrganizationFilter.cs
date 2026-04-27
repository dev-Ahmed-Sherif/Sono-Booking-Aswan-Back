using SonoTracker.Domain.Enum;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.Organization.Parameters
{
    [ExcludeFromCodeCoverage]
    public class OrganizationFilter
    {
        public string Code { get; set; }
        public string NameAr { get; set; }=string.Empty;

        public string NameEn { get; set; }= string.Empty;

        public int? TouristMarinaNumber { get; set; }

        public OrganizationType? OrganizationType { get; set; }

        public string OrganizationCategoryId { get; set; }
        public Guid? InspectionTypeId { get; set; }

        public bool IsDeleted { get; set; } = true;
    }
}
