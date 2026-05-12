using System;
using System.ComponentModel.DataAnnotations;
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

    [Required, MaxLength(20)]
    public required string NationalId { get; set; }
}
