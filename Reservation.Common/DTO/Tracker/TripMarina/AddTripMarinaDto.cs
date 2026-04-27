using SonoTracker.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.TripMarina
{
    public class AddTripMarinaDto : IEntityDto<string>
    {
        public string? Id { get; set; }

        public string TouristMarinaId { get; set; }

        public string TripInformationId { get; set; }

    }
}
