using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;

namespace SonoBooking.Domain.Entities.Lookups;

public class Employee : BaseAudit<string>
{
    public Employee()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }

    [Required, MaxLength(280)]
    public required string Name { get; set; }

    [Required, MaxLength(20)]
    public required string NationalId { get; set; }

    [MaxLength(50), ForeignKey(nameof(EmployeeOrg))]
    public string? EmployeeOrgId { get; set; }
    public virtual EmployeeOrg? EmployeeOrg { get; set; }

    [MaxLength(50), ForeignKey(nameof(EmployeeJob))]
    public string? EmployeeJobId { get; set; }
    public virtual EmployeeJob? EmployeeJob { get; set; }
}
