using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;

namespace SonoBooking.Domain.Entities.Housing;

public class Bed : BaseAudit<string>
{
    public Bed()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }

    [Required, MaxLength(20)]
    public required string BedNumber { get; set; }

    [Required, MaxLength(250)]
    public required string Description { get; set; }

    [Required, MaxLength(100)]
    public required string Dimensions { get; set; }

    [Required]
    public required decimal Price { get; set; }

    [Required]
    public required UnitStatus Status { get; set; } = UnitStatus.Available;

    [Required, MaxLength(50)]
    [ForeignKey(nameof(Room))]
    public required string RoomId { get; set; }
    public virtual Room? Room { get; set; }

    public virtual ICollection<RequestUnit> RequestUnits { get; set; } = [];
    public virtual ICollection<UnitImage> UnitImages { get; set; } = [];
}
