using System.ComponentModel.DataAnnotations;

namespace SonoBooking.Common.DTO.Identity.User
{
    public class LoginRequestDto
    {
        [Required, EmailAddress] 
        public required string Email { get; set; }

        [Required] 
        public required string Password { get; set; }
    }
}

