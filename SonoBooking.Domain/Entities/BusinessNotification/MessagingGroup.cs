using SonoBooking.Domain.Entities.Lookups;
using SonoTracker.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoBooking.Domain.Entities.BusinessNotification
{
    public class MessagingGroup : Lookup<string>
    {
        public MessagingGroup()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        public virtual HashSet<Message> Messages { get; set; } = [];

    }
}
