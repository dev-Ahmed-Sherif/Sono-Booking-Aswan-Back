using System;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Domain.Enum;

namespace SonoTracker.Common.DTO.Tracker.AccidentOrganization.Parameters
{
    [ExcludeFromCodeCoverage]

    public class AccidentOrganizationFilter
    {
        public string OrganizationId { get; set; }

        public string AccidentId { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}
