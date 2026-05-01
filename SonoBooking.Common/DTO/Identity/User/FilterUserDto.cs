using System;

namespace SonoTracker.Common.DTO.Identity.User
{
    public class FilterUserDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}
