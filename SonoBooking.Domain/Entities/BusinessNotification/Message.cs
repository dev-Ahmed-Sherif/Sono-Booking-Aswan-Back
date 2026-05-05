using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Domain.Entities.BusinessNotification
{
    [ExcludeFromCodeCoverage]

    public class Message : BaseAudit<string>
    {
        public Message()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [Required, MaxLength(14000)]
        public required string Content { get; set; }

        [Required]
        [MaxLength(50), ForeignKey(nameof(Sender))]
        public required string SenderId { get; set; }
        public virtual User? Sender { get; set; }

        [MaxLength(50), ForeignKey(nameof(Receiver))]
        public required string ReceiverId { get; set; }
        public virtual User? Receiver { get; set; }

        [MaxLength(50), ForeignKey(nameof(MessagingGroup))]
        public string? MessagingGroupId { get; set; }
        public virtual MessagingGroup? MessagingGroup { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }
        public bool IsRead { get; set; }
    }
}
