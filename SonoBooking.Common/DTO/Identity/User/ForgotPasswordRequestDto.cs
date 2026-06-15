using System.ComponentModel.DataAnnotations;

namespace SonoBooking.Common.DTO.Identity.User
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        public required string Identifier { get; set; }
    }
}
