using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Domain.Entities.Housing;

public class Apartment : BaseAudit<string>
{
    public Apartment()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }

    [MaxLength(20)]
    public required string ApartmentNumber { get; set; }

    [MaxLength(250)]
    public required string Description { get; set; }

    [Required]
    public required decimal Price { get; set; }

    [Required]
    public required UnitStatus Status { get; set; } = UnitStatus.Available;

    [Required]
    public required Gender Gender { get; set; }

    [Required]
    public required AllocationType AllocationType { get; set; } = AllocationType.Fixed;

    [Required, MaxLength(50)]
    public required string Street { get; set; }

    [Required, MaxLength(50)]
    public required string BuildingNumber { get; set; }

    [Required, MaxLength(50)]
    public required string Floor { get; set; }

    [Required, MaxLength(500)]
    public required string DetailedAddress { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(ApartmentType))]
    public required string ApartmentTypeId { get; set; }
    public virtual ApartmentType? ApartmentType { get; set; }
    
    [Required, MaxLength(50)]
    [ForeignKey(nameof(Governorate))]
    public required string GovernorateId { get; set; }
    public virtual Governorate? Governorate { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(City))]
    public required string CityId { get; set; }
    public virtual City? City { get; set; }

    public virtual ICollection<RequestUnit> RequestUnits { get; set; } = [];

    public virtual ICollection<Reservation> Reservations { get; set; } = [];

    public virtual ICollection<Room> Rooms { get; set; } = [];

    public virtual ICollection<UnitImage> UnitImages { get; set; } = [];
}
