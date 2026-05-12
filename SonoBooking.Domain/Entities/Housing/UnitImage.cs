using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Domain.Entities.Housing;

public class UnitImage : BaseEntity<string>
{
    public UnitImage()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }

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

    [Required, MaxLength(50)]
    [ForeignKey(nameof(Attachment))]
    public required string AttachmentId { get; set; }
    public virtual Attachment? Attachment { get; set; }

    public bool IsPrimary { get; set; }
}
