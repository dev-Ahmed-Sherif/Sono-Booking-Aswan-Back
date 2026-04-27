using SonoTracker.Common.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace SonoTracker.Common.DTO.Identity.User
{
    public class UpdateUserDto : IEntityDto<string>
    {
        [Required, MaxLength(50)]
        public required string Id { get; set; }
        public string UserName { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }
        
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }
        
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
        
        public Guid RoleId { get; set; }
        
        public Guid? OrganizationId { get; set; }
    }
}
