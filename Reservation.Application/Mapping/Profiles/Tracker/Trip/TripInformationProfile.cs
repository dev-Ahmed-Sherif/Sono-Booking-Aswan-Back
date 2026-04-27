using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Tracker.FloatingUnit;
using SonoTracker.Common.DTO.Tracker.TripInformation;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapTripInformation()
        {
            CreateMap<TripInformation, TripInformationDto>()
                 .ForMember(des => des.RouteName, opt => opt.MapFrom(src => src.Route.NameAr))
                 .ForMember(des => des.FloatingUnitNameAr, opt => opt.MapFrom(src => src.FloatingUnit.NameAr))
                 .ForMember(des => des.FloatingUnitNameEn, opt => opt.MapFrom(src => src.FloatingUnit.NameEn))
                 .ForMember(des => des.FloatingUnitCode, opt => opt.MapFrom(src => src.FloatingUnit.Code)).ReverseMap();
            CreateMap<TripInformation, EditTripInformationDto>().ReverseMap();
            CreateMap<AddTripInformationDto, TripInformation>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
