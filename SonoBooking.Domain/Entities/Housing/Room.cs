using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Domain.Entities.Housing;

public class Room : BaseAudit<string>
{
    public Room()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }

    [Required, MaxLength(50)]
    public required string RoomNumber { get; set; }

    [Required, MaxLength(500)]
    public required string Description { get; set; }

    [Required]
    public required decimal Price { get; set; }

    [Required]
    public required UnitStatus Status { get; set; } = UnitStatus.Available;

    [Required, MaxLength(50)]
    [ForeignKey(nameof(Apartment))]
    public required string ApartmentId { get; set; }
    public virtual Apartment? Apartment { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(RoomType))]
    public required string RoomTypeId { get; set; }
    public virtual RoomType? RoomType { get; set; }

    public virtual ICollection<RequestUnit> RequestUnits { get; set; } = [];

    public virtual ICollection<Reservation> Reservations { get; set; } = [];

    public virtual ICollection<UnitImage> UnitImages { get; set; } = [];

    public virtual ICollection<Bed> Beds { get; set; } = [];
}
