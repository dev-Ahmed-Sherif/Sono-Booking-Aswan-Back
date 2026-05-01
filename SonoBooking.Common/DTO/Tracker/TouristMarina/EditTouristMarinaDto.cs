using SonoTracker.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.TouristMarina
{
    public class EditTouristMarinaDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; }
        public string TownId { get; set; }
        public string TownNameAr { get; set; }
        public string TownNameEn { get; set; }
        public string Url { get; set; }
        public float Length { get; set; }
        public string NorthSide { get; set; }
        public string SouthSide { get; set; }
        public string GeoPointId { get; set; }
        public string GeoPointNorth { get; set; }
        public string GeoPointEast { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}
