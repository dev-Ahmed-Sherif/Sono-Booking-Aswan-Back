using System;

namespace SonoTracker.Common.DTO.Tracker.OrganizationStaff.Parameters
{
    public class OrganizationStaffFilter
    {
        public string Name { get; set; } = string.Empty;

        public string Job { get; set; } = string.Empty;

        public string? OrganizationId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public bool IsDelegate { get; set; } = false;

    }
}
