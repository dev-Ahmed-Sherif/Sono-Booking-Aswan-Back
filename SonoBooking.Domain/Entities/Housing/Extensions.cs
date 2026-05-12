using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Housing;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Domain.Entities.Housing;

public class Extension : BaseAudit<string>
{
    public Extension()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }

    [Required]
    public required DateOnly EndDate { get; set; }

    [Required]
    public required Status Status { get; set; } = Status.Pending;
    
    [Required]
    public required AllocationType ExtensionAllocationType { get; set; }

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(Reservation))]
    public required string ReservationId { get; set; }
    public virtual Reservation? Reservation { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(User))]
    public required string UserId { get; set; }
    public virtual User? User { get; set; }

    [MaxLength(50)]
    [ForeignKey(nameof(ApprovedBy))]
    public string? ApprovedById { get; set; }
    public virtual User? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }
}
