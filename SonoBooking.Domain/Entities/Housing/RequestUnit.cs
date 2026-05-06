using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;

namespace SonoBooking.Domain.Entities.Housing;

public class RequestUnit : BaseAudit<string>
{
    public RequestUnit()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(Request))]
    public required string RequestId { get; set; }
    public virtual Request? Request { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(Apartment))]
    public string? ApartmentId { get; set; }
    public virtual Apartment? Apartment { get; set; }

    [MaxLength(50)]
    [ForeignKey(nameof(Bed))]
    public string? BedId { get; set; }
    public virtual Bed? Bed { get; set; }

    [MaxLength(50)]
    [ForeignKey(nameof(Room))]
    public string? RoomId { get; set; }
    public virtual Room? Room { get; set; }
}
