using SonoBooking.Domain.Entities.Business;
using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoBooking.Domain.Entities.Identity
{
    public class UserRelative : Lookup<string>
    {
        public UserRelative() 
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
        
        [Required]
        public required Gender Gender { get; set; }
        
        [Required]
        public required DateOnly BirthDate { get; set; }
        
        [Required, MaxLength(14)]
        public required string NationalId { get; set; }
        
        [Required, MaxLength(140)]
        public required string NationalIdImage { get; set; }

        [Required,  MinLength(14)]
        public required string Phone { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(User))]
        public required string UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
