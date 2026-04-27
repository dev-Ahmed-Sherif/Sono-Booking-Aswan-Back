
using Microsoft.AspNetCore.Identity;
using SonoTracker.Domain.Entities.Lookups;
using SonoTracker.Domain.Entities.Tracker;
using SonoTracker.Domain.Entities.TrackerNotification;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Domain.Entities.Identity
{
    public class User : IdentityUser
    {
        public User()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [MaxLength(50)]
        public override string Id { get; set; }

        [Required, MaxLength(70)]
        public required string FullName { get; set; }

        public bool IsLogedIn { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required, MaxLength(50)]
        public required string CreatedById { get; set; }

        [Required, MaxLength(70)]
        public required string CreatedBy { get; set; }

        public DateTime ModifiedAt { get; set; }

        [Required, MaxLength(50)]
        public required string ModifiedById { get; set; }

        [Required, MaxLength(70)]
        public required string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }

        [MaxLength(50), ForeignKey(nameof(Organization))]
        public string? OrganizationId { get; set; }
        public virtual Organization? Organization { get; set; }

        [MaxLength(50), ForeignKey(nameof(FloatingUnit))]
        public string? FloatingUnitId { get; set; }
        public virtual FloatingUnit? FloatingUnit { get; set; }

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        public virtual HashSet<RefreshToken> RefreshTokens { get; set; } = [];
        public virtual HashSet<Message> Messages { get; set; } = [];
        public virtual HashSet<Notification> Notifications { get; set; } = [];
    }
}
