using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;
using SonoTracker.Domain.Enum;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Tracker.FloatingUnitStaff
{
    [ExcludeFromCodeCoverage]

    public class AddFloatingUnitStaffDto : IEntityDto<string>
    {
        public string Id { get; set; }

        [Required, RegularExpression("^[ \u0600-\u06FF\u0750-\u077Fa-zA-Z]+$",
         ErrorMessage = "Name No Numbers Or Special Chars Allowed")]
        public string Name { get; set; }
        
        [Required, RegularExpression("^[ \u0600-\u06FF\u0750-\u077Fa-zA-Z]+$",
         ErrorMessage = "Job No Numbers Or Special Chars Allowed")]
        public string Job { get; set; }
        
        [Required, RegularExpression("^[0-9]{11}$",
         ErrorMessage = "Mobile No Numbers Only And 11 Digits")]
        public string Mobile { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public Gender Gender { get; set; }
        
        [Required]
        public IDType IDType { get; set; }
        
        [Required, RegularExpression("^[0-9A-Za-z]{7,15}$",
         ErrorMessage = "Identity Must Be 15 Digits")]
        public string Identity { get; set; }
        
        [Required, MaxLength(50)]
        public string NationalityId { get; set; }

        [Required, MaxLength(50)]
        public string FloatingUnitId { get; set; }
        public bool IsDelegate { get; set; }
        public IFormFile DelegateAttachment { get; set; }
    }
}
