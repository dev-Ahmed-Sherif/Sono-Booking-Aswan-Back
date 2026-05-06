using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Housing;
using SonoBooking.Domain.Entities.Identity;

namespace SonoBooking.Domain.Entities.Housing;

public class RequestParticipant : BaseAudit<string>
{
    public RequestParticipant()
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
    [ForeignKey(nameof(Companion))]
    public required string CompanionId { get; set; }
    public virtual Companion? Companion { get; set; }

}
