using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.GovernorateGeoPoint.Parameters
{
    [ExcludeFromCodeCoverage]
    public class GovernorateGeoPointFilter
    {
        public Guid? GeoPointId { get; set; }

        public Guid? GovernorateId { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
