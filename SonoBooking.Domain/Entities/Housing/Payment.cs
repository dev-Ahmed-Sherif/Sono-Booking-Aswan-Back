using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Domain.Entities.Housing;

public class Payment : BaseAudit<string>
{
    public Payment()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }

    [Required]
    public required decimal Amount { get; set; }

    [Required, MaxLength(50)]
    public required PaymentMethod PaymentMethod { get; set; }

    [Required, MaxLength(50)]
    public required PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    [Required]
    public required DateTime PaymentDate { get; set; }

    [Required, MaxLength(100)]
    public required string TransactionReference { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(Reservation))]
    public required string ReservationId { get; set; }
    public virtual Reservation? Reservation { get; set; }
}
