using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Domain.Entities.Lookups
{
    [ExcludeFromCodeCoverage]
    public class Governorate : Lookup<string>
    {
        public Governorate()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [Required, MaxLength(100)]
        public required string WebsiteUrl { get; set; }

        [Required, MaxLength(250)]
        public required string Address { get; set; }

        [Required, MaxLength(250)]
        public required string ImageUrl { get; set; }
       
        public virtual HashSet<City> Cities { get; set; } = [];

    }
}
