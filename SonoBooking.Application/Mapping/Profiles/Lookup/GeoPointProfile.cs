using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Lookup.AccidentType;
using SonoTracker.Common.DTO.Lookup.GeoPoint;
using SonoTracker.Domain.Entities.Lookups;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapGeoPoint()
        {
            CreateMap<GeoPoint, GeoPointDto>().ReverseMap();
            
            CreateMap<GeoPoint, EditGeoPointDto>().ReverseMap();

            CreateMap<AddGeoPointDto, GeoPoint>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
