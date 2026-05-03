
using Microsoft.AspNetCore.Identity;
using SonoBooking.Domain.Entities.BusinessNotification;
using SonoBooking.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoBooking.Domain.Entities.Identity
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

        [Required]
        public required Gender Gender { get; set; }

        [Required]
        public required DateOnly BirthDate { get; set; }

        [Required, MaxLength(14)]
        public required string NationalId { get; set; }

        [Required, MaxLength(140)]
        public required string NationalIdImage { get; set; }

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

        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        public virtual HashSet<RefreshToken> RefreshTokens { get; set; } = [];
        public virtual HashSet<Message> Messages { get; set; } = [];
        public virtual HashSet<Notification> Notifications { get; set; } = [];
    }
}
