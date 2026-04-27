using System;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Domain.Enum;

namespace SonoTracker.Common.DTO.Tracker.FloatingUnitOrganization.Parameters
{
    [ExcludeFromCodeCoverage]

    public class FloatingUnitOrganizationFilter
    {
        public string? OrganizationId { get; set; }
        public OrganizationType? OrganizationType { get; set; }

        public string? FloatingUnitId { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}
