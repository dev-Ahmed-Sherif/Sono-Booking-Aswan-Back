using SonoTracker.Common.DTO.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace SonoTracker.Common.DTO.Lookup.Town
{
    public class AddTownDto : LookupDto<string>
    {
        [Required]
        public required string CityId { get; set; }
    }
}
