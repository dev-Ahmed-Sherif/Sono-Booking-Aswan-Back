using SonoTracker.Common.DTO.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace SonoTracker.Common.DTO.Lookup.City
{
    public class AddCityDto : LookupDto<string>
    {
        [Required]
        public required string GovernateId { get; set; }
    }
}
