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

    public string RequestNumber { get; set; } = string.Empty;

    public DateTime RequestDate { get; set; } = DateTime.UtcNow;

    public required DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public Status Status { get; set; } = Status.Pending;

    public string RejectionReason { get; set; } = string.Empty;

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
