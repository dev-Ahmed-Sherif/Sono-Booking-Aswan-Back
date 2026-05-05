using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;

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
    [ForeignKey(nameof(Apartment))]
    public required string ApartmentId { get; set; }
    public virtual Apartment? Apartment { get; set; }

    [Required, MaxLength(50)]
    public required string RoomNumber { get; set; }

    public string RoomType { get; set; }
    public string Description { get; set; } = string.Empty;

    [Required]
    public required decimal Price { get; set; }

    public UnitStatus Status { get; set; }

    public virtual ICollection<RequestUnit> RequestUnits { get; set; } = [];

    public virtual ICollection<Reservation> Reservations { get; set; } = [];

    public virtual ICollection<UnitImage> UnitImages { get; set; } = [];

    public virtual ICollection<Bed> Beds { get; set; } = [];
}
