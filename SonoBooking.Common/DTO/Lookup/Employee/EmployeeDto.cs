using SonoBooking.Common.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Lookup.Employee
{
    [ExcludeFromCodeCoverage]
    public class EmployeeDto : IEntityDto<string>
    {
        public string Id { get; set; }

        [Required, MaxLength(280)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string NationalId { get; set; }

        public string EmployeeOrgId { get; set; }
        public string EmployeeJobId { get; set; }
        public string EmployeeOrg { get; set; }
        public string EmployeeJob { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
