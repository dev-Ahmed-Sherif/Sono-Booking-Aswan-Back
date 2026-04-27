using System.ComponentModel.DataAnnotations;

namespace SonoTracker.Common.DTO.Identity.User
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        public string Username { get; set; }
        //[Required]
        public string RoleId { get; set; }
        public string OrgId { get; set; }
        public string FloatingUnitId { get; set; }
        public string GovernorateId { get; set; }
    }
}
