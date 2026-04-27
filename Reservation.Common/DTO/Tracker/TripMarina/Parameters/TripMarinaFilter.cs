using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.TripMarina.Parameters
{
    [ExcludeFromCodeCoverage]
    public  class TripMarinaFilter 
    {
        public string? TouristMarinaId { get; set; }
        public string? TripInformationId { get; set; }
        public string? FloatingUnitId { get; set; }
        public bool IsDeleted { get; set; } = false;


    }
}
