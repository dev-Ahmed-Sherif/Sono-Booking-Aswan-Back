using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;

namespace SonoTracker.Common.DTO.Tracker.Governorate
{
    [ExcludeFromCodeCoverage]
    public class AddGovernorateDto : IEntityDto<string>
    {
        public string Id { get; set; }

        [MaxLength(100),Required]
        public required string NameAr { get; set; }
        
        [MaxLength(100), Required]
        public required string NameEn { get; set; }

        [MaxLength(2), Required, RegularExpression("^[0-9]+$",ErrorMessage = "Must be Number")]
        public required string Code { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }

        [RegularExpression("^(https?://)?(www\\.)?([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,}\\/?([^\\s]*)$", ErrorMessage = "Unvalid Web Site Address")]
        public string WebsiteUrl { get; set; }

        public IFormFile ImageUrl { get; set; }
    }
}
