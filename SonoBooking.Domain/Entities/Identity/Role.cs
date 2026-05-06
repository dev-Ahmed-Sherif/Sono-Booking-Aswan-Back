using Microsoft.AspNetCore.Identity;
using SonoBooking.Domain.Entities.Lookups;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoBooking.Domain.Entities.Identity
{
    public class Role : IdentityRole
    {
        public Role()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [MaxLength(50)]
        public override string Id { get; set; }

        [Required,MaxLength(70)]
        public required string NameAr { get; set; }

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
    }
}
