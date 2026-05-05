using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Identity;

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

    public ReservationStatus Status { get; set; } = ReservationStatus.Reserved;

    [Required]
    public required decimal UnitPrice { get; set; }
    [Required]
    public required decimal TotalAmount { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(User))]
    public required string UserId { get; set; }
    public virtual User? User { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(Request))]
    public required string RequestId { get; set; }
    public virtual Request? Request { get; set; }

    [MaxLength(50)]
    [ForeignKey(nameof(Apartment))]
    public string? ApartmentId { get; set; }
    public virtual Apartment? Apartment { get; set; }

    [MaxLength(50)]
    [ForeignKey(nameof(Room))]
    public string? RoomId { get; set; }
    public virtual Room? Room { get; set; }

    [MaxLength(50)]
    [ForeignKey(nameof(Bed))]
    public string? BedId { get; set; }
    public virtual Bed? Bed { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = [];

}
