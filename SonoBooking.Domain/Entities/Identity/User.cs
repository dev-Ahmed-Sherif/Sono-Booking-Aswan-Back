
using Microsoft.AspNetCore.Identity;
using SonoBooking.Domain.Entities.BusinessNotification;
using SonoBooking.Domain.Entities.Housing;
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

        public Gender? Gender { get; set; }

        public DateOnly? BirthDate { get; set; }

        [MaxLength(20)]
        public string? DocumentNumber { get; set; }

        public IDType? DocumentType { get; set; }

        [MaxLength(140)]
        public string? DocumentImageUrl { get; set; }

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

        [MaxLength(50)]
        [ForeignKey(nameof(Leader))]
        public string? LeaderId { get; set; }
        public virtual Leader? Leader { get; set; }

        [MaxLength(50)]
        [ForeignKey(nameof(EmployeeId))]
        public string? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        [MaxLength(280)]
        public string? JobTitle { get; set; }

        [MaxLength(280)]
        public string? Organization { get; set; }

        public virtual HashSet<RefreshToken> RefreshTokens { get; set; } = [];
        public virtual HashSet<Message> Messages { get; set; } = [];
        public virtual HashSet<Notification> Notifications { get; set; } = [];

        public virtual HashSet<Companion> Companions { get; set; } = [];
        public virtual HashSet<Request> Requests { get; set; } = [];
    }
}
