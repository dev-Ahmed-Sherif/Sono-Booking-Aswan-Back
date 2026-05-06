using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Housing;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Domain.Entities.Housing;

public class Request : BaseAudit<string>
{
    public Request()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }


    [Required, MaxLength(30)]
    public required string RequestNumber { get; set; }

    [Required]
    public required DateTime RequestDate { get; set; }

    [Required]
    public required DateOnly StartDate { get; set; }

    [Required]
    public required DateOnly EndDate { get; set; }

    [Required]
    public required Status Status { get; set; } = Status.Pending;

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(RequestType))]
    public required string RequestTypeId { get; set; }
    public virtual RequestType? RequestType { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(User))]
    public required string UserId { get; set; }
    public virtual User? User { get; set; }

    [MaxLength(50)]
    [ForeignKey(nameof(ApprovedBy))]
    public string? ApprovedById { get; set; }
    public virtual User? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public virtual ICollection<RequestParticipant> RequestParticipants { get; set; } = [];

    public virtual ICollection<RequestUnit> RequestUnits { get; set; } = [];

    public virtual ICollection<Reservation> Reservations { get; set; } = [];
    public virtual ICollection<Approval> Approvals { get; set; } = [];
}
