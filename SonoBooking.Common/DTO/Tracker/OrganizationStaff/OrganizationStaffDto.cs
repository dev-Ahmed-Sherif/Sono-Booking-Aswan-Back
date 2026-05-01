using SonoTracker.Common.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.OrganizationStaff
{
    [ExcludeFromCodeCoverage]
    public class OrganizationStaffDto : IEntityDto<string>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Job { get; set; }

        public string Phone { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string PhoneExtension { get; set; }

        public string NationalId { get; set; }

        public string OrganizationId { get; set; }
        
        public string OrganizationName { get; set; }

        public bool IsDelegate { get; set; }

        public string DelegateAttachment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}
