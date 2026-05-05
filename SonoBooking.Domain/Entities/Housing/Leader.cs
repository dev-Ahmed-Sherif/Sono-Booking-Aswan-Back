using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SonoBooking.Domain.Entities.Base;

namespace SonoBooking.Domain.Entities.Housing;

public class Leader : BaseAudit<string>
{
    public Leader()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }

    [Required, MaxLength(50)]
    public required string FullName { get; set; }

    [Required, MaxLength(50)]
    public required string Position { get; set; }

    [Required]
    public required bool IsActive { get; set; }


    public virtual ICollection<Approval> Approvals { get; set; } = new List<Approval>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
