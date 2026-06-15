using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Domain.Entities.Housing;

public class RequestAttach : BaseEntity<string>
{
    public RequestAttach()
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
    [ForeignKey(nameof(Attachment))]
    public required string AttachmentId { get; set; }
    public virtual Attachment? Attachment { get; set; }

    public bool IsPrimary { get; set; }
}
