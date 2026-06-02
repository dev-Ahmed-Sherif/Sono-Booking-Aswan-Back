using Microsoft.AspNetCore.Http;
using SonoBooking.Common.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace SonoBooking.Common.DTO.Identity.User
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

        /// <summary>Optional identity document image; when set, replaces the stored document file (same flow as MaintenanceService upload).</summary>
        public IFormFile DocumentImage { get; set; }
        
        public Guid? OrganizationId { get; set; }
    }
}


