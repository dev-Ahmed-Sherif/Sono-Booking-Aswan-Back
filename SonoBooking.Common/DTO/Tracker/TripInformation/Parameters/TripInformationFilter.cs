using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.TripInformation.Parameters
{
    public class TripInformationFilter
    {
        public DateTime? SartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Code { get; set; }

        public string? FloatingUnitId { get; set; }

        public string? RouteId { get; set; }

        public bool IsDeleted { get; set; } = false;

    }
}
