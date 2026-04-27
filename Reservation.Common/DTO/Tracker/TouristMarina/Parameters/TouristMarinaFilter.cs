using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.TouristMarina.Parameters
{
    [ExcludeFromCodeCoverage]
    public  class TouristMarinaFilter
    {
        public string Code { get; set; }
        public string CityId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;


    }
}
