using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Domain.Entities.Tracker
{
    [ExcludeFromCodeCoverage]
    public class TripMarina : BaseEntity<string>
    {
        public TripMarina()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [MaxLength(50), ForeignKey(nameof(TouristMarina))]
        public string? TouristMarinaId { get; set; }
        public virtual TouristMarina? TouristMarina { get; set; }

        [MaxLength(50), ForeignKey(nameof(TripInformation))]
        public string? TripInformationId { get; set; }
        public virtual TripInformation? TripInformation { get; set; }
    }
}
