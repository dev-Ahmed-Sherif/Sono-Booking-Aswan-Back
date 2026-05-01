using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Identity;
using SonoTracker.Domain.Entities.Lookups;

namespace SonoTracker.Domain.Entities.TrackerNotification
{
    [ExcludeFromCodeCoverage]
    public class Notification : BaseAudit<string>
    {
        public Notification()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [Required, MaxLength(14000)]
        public  required string Content { get; set; }
        public bool IsRead { get; set; }

        [Required]
        [MaxLength(50), ForeignKey(nameof(Sender))]
        public required string SenderId { get; set; }
        public virtual User? Sender { get; set; }

        [MaxLength(50), ForeignKey(nameof(Receiver))]
        public required string ReceiverId { get; set; }
        public virtual User? Receiver { get; set; }

        [MaxLength(50), ForeignKey(nameof(NotificationGroup))]
        public string? NotificationGroupId { get; set; }
        public virtual NotificationGroup? NotificationGroup { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }
    }
}
