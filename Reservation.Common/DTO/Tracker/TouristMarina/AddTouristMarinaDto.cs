using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;

namespace SonoTracker.Common.DTO.Tracker.TouristMarina
{
    public class AddTouristMarinaDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public required string NameAr { get; set; }
        public string NameEn { get; set; }
        public required string MarinaAddress { get; set; }
        public required string CityId { get; set; }
        public required float Length { get; set; }
        public required string NorthSide { get; set; }
        public required string SouthSide { get; set; }
        public string NorthGeo { get; set; }
        public string EastGeo { get; set; }
        public string GeoPointId { get; set; }
        public string Note { get; set; }
        public IFormFile ImageUrl { get; set; }
    }
}
