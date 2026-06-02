using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Base;

namespace SonoBooking.Domain.Entities.Housing;

public class Reservation : BaseAudit<string>
{
    public Reservation()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }
    
    [Required]
    public required DateOnly StartDate { get; set; }

    [Required]
    public required DateOnly EndDate { get; set; }

    public DateTime? CheckInDate { get; set; }

    public DateTime? ActualCheckOutDate { get; set; }

    [Required]
    public required ReservationStatus Status { get; set; } = ReservationStatus.Reserved;

    [Required]
    public required decimal TotalAmount { get; set; }
    [MaxLength(700)]
    public string CancelationReason { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(Request))]
    public required string RequestId { get; set; }
    public virtual Request? Request { get; set; }

    public virtual Payment? Payment { get; set; }

}
