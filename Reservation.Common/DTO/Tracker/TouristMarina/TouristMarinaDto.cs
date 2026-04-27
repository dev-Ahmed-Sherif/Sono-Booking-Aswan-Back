using SonoTracker.Common.Core;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.TouristMarina
{
    public class TouristMarinaDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string CityId { get; set; }
        public string CityNameAr { get; set; }
        public string CityNameEn { get; set; }
        public string MarinaAddress { get; set; }
        public float Length { get; set; }
        public string NorthSide { get; set; }
        public string SouthSide { get; set; }
        public string GeoPointId { get; set; }
        public string NorthGeo { get; set; }
        public string EastGeo { get; set; }
        public string Note { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
