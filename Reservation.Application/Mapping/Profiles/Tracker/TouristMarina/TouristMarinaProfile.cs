using SonoTracker.Common.DTO.Tracker.TouristMarina;
using SonoTracker.Domain.Entities.Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapTouristMarina()
        {
            CreateMap<TouristMarina, TouristMarinaDto>()
                .ForMember(des => des.NorthGeo, opt => opt.MapFrom(src => src.GeoPoint.North))
                .ForMember(des => des.EastGeo, opt => opt.MapFrom(src => src.GeoPoint.East))
                .ReverseMap();
            CreateMap<TouristMarina, EditTouristMarinaDto>().ReverseMap();
            CreateMap<AddTouristMarinaDto, TouristMarina>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
