using System;
using System.Collections.Generic;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Domain.Entities.Housing;

public partial class Payment
{
    public int Id { get; set; }

    public int ReservationId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; }

    public string PaymentStatus { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string TransactionReference { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Reservation Reservation { get; set; }
}
