using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Domain.Entities.Housing;

public class Companion : BaseAudit<string>
{
    public Companion()
    {
        if (string.IsNullOrEmpty(Id))
        {
            Id = Guid.CreateVersion7().ToString();
        }
    }

    [Required, MaxLength(100)]
    public required string FullName { get; set; }
    
    [Required]
    public required Gender Gender { get; set; }

    [Required]
    public required DateOnly BirthDate { get; set; }

    [Required, MaxLength(140)]
    public required string DocumentImageUrl { get; set; }

    [Required, MaxLength(20)]
    public required string DocumentNumber { get; set; }

    [Required]
    public required IDType DocumentType { get; set; }
    
    [Required, MaxLength(50)]
    [ForeignKey(nameof(User))]
    public required string UserId { get; set; }
    public virtual User? User { get; set; }

    [Required, MaxLength(50)]
    [ForeignKey(nameof(Relationship))]
    public required string RelationshipId { get; set; }
    public virtual Relationship? Relationship { get; set; }

    public virtual ICollection<RequestParticipant> RequestParticipants { get; set; } = [];
}
