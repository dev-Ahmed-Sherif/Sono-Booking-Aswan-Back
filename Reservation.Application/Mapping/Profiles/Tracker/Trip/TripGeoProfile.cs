using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Tracker.FloatingUnit;
using SonoTracker.Common.DTO.Tracker.TripGeo;
using SonoTracker.Common.DTO.Tracker.TripInformation;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapTripGeo()
        {
            CreateMap<TripGeo, TripGeoDto>()
                .ForMember(des => des.FloatingUnitNameAr, 
                           opt => opt
                           .MapFrom(src => src.TripInformation.FloatingUnit.NameAr
                           )).ReverseMap();
            CreateMap<TripGeo, EditTripGeoDto>()
                .ForMember(des => des.FloatingUnitNameAr,
                           opt => opt
                           .MapFrom(src => src.TripInformation.FloatingUnit.NameAr
                           ))
                .ReverseMap();
            CreateMap<AddTripGeoDto, TripGeo>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
