using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Tracker.AccidentOrganization.Parameters
{
    [ExcludeFromCodeCoverage]

    public class AccidentOrganizationFilter
    {
        public string OrganizationId { get; set; }

        public string AccidentId { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}

