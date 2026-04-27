using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonoTracker.Common.Core;

namespace SonoTracker.Common.DTO.Tracker.GovernorateGeoPoint
{
    [ExcludeFromCodeCoverage]
    public class GovernorateGeoPointDto : IEntityDto<string>
    {
        public string? Id { get; set; }

        public Guid GeoPointId { get; set; }

        public string GeoPointNorth { get; set; }

        public string GeoPointEast { get; set; }

        public Guid GovernorateId { get; set; }

        public string GovernorateName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}
