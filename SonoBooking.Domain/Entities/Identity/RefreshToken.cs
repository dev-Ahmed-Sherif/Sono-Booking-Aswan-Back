using SonoBooking.Domain.Entities.Base;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoBooking.Domain.Entities.Identity
{
    public class RefreshToken : BaseAudit<string>
    {
        public RefreshToken()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
        [Required, MaxLength(70)]
        public required string Token { get; set; }
        public DateTime ExpiryTime { get; set; }

        [Required, MaxLength(50)]  
        [ForeignKey(nameof(User))]
        public required string UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
