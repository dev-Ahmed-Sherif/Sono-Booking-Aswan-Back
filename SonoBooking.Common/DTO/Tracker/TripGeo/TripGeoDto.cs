using SonoTracker.Common.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.TripGeo
{
    public class TripGeoDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string GeoPointId { get; set; }
        public string GeoPointNorth { get; set; }
        public string GeoPointEast { get; set; }
        public string TripInformationId { get; set; }
        public string TripInformationCode { get; set; }
        public string FloatingUnitNameAr { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}
