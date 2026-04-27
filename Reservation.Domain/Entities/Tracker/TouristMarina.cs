using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class TouristMarina : Lookup<string>
    {
        public TouristMarina()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
        [Required, MaxLength(250)]
        public required string MarinaAddress { get; set; }
        [Required]
        public required float Length { get; set; }
        [Required, MaxLength(50)]
        public required string NorthSide { get; set; }
        [Required, MaxLength(50)]
        public required string SouthSide { get; set; }
        [MaxLength(250)]
        public string? Note { get; set; }
        [MaxLength(250)]
        public string? ImageUrl { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(City))]
        public required string CityId { get; set; }
        public virtual City? City { get; set; }

        [MaxLength(50), ForeignKey(nameof(GeoPoint))]
        public string? GeoPointId { get; set; }
        public virtual GeoPoint? GeoPoint { get; set; }
        
        [MaxLength(50), ForeignKey(nameof(Governorate))]
        public string? GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }

        public virtual HashSet<TouristMarinaOrganization> TouristMarinaOwners { get; set; } = [];
        public virtual HashSet<TripMarina> TripMarinas { get; set; } = [];
    }
}
