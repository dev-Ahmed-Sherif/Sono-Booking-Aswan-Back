using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Tracker;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonoTracker.Domain.Entities.Lookups
{
    public class GeoPoint : Lookup<string>
    {
        public GeoPoint()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [Required, MaxLength(50)]
        public required string North { get; set; }

        [Required, MaxLength(50)]
        public required string East { get; set; }

        [MaxLength(50)]
        [ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        public virtual HashSet<Accident> Accidents { get; set; } = [];
        public virtual HashSet<TouristMarina> TouristMarinas { get; set; } = [];
        public virtual HashSet<TripGeo> TripGeos { get; set; } = [];
    }
}
