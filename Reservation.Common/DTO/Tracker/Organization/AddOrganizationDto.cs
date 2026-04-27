using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;
using SonoTracker.Domain.Enum;

namespace SonoTracker.Common.DTO.Tracker.Organization
{
    [ExcludeFromCodeCoverage]
    public class AddOrganizationDto : IEntityDto<string>
    {
        public string Id { get; set; }

        //[RegularExpression("^[0-9]+$", ErrorMessage = "Must be number")]
        //public string Code { get; set; }

        [Required,MaxLength(100), RegularExpression("^[\u0600-\u06FF\\s]+$",ErrorMessage = "Arabic Name Should be Arabic")]
        public required string NameAr { get; set; }

        [MaxLength(100), RegularExpression("^[a-zA-Z\\s]+$", ErrorMessage = "English Name Should be English")]
        public string NameEn { get; set; }
        
        [Required, MaxLength(200)]
        public required string Address { get; set; }
        
        [Required,Phone]
        public string Phone { get; set; }
        
        [RegularExpression("^(?:\\+20|0)?[1-9][0-9]{1}[0-9]{7}$")]
        public string Fax { get; set; }
        
        [Required,Phone]
        public string Mobile { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
        
        public string NationalityId { get; set; }

        public string OrganizationCategoryId { get; set; }

        [Required]
        public required OrganizationType OrganizationType { get; set; }
        
        [RegularExpression("^(https?://)?(www\\.)?([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,}\\/?([^\\s]*)$",ErrorMessage = "Unvalid Web Site Address")]
        public string WebSiteAddress { get; set; }
        
        [RegularExpression("^[0-9]{1,2}$", ErrorMessage = "Must be number and length less than 3")]
        public int? TouristMarinaNumber { get; set; }
        
        public bool IsReport { get; set; }
        
        public bool IsAccepted { get; set; }
        
        [
            MaxLength(15),
            RegularExpression("^[0-9]{1,15}$", ErrorMessage = "Must be number and length less than 16")
        ]
        public string CommercialRegistrationNumber { get; set; }
        
        public IFormFile CommercialRegistrationAttachment { get; set; }
    }
}
