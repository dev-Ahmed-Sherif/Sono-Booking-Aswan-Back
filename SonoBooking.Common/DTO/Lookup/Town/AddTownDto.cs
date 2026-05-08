using SonoBooking.Common.DTO.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace SonoBooking.Common.DTO.Lookup.Town
{
    public class AddTownDto : LookupDto<string>
    {
        [Required]
        public required string CityId { get; set; }
    }
}


