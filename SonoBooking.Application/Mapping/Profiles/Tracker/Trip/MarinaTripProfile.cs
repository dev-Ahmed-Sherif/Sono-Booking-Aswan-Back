using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Tracker.GeneralInspection;
using SonoTracker.Common.DTO.Tracker.Inspection;
using SonoTracker.Common.DTO.Tracker.TripInformation;
using SonoTracker.Common.DTO.Tracker.TripMarina;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapMarinaTrip()
        {
            CreateMap<TripMarina, TripMarinaDto>()
                 .ForMember(des => des.TouristMarinaName, opt => opt.MapFrom(src => src.TouristMarina.NameAr))
                 .ForMember(des => des.TouristMarinaCode, opt => opt.MapFrom(src => src.TouristMarina.Code))
                 .ForMember(des => des.FloatingUnitNameAr, opt => opt.MapFrom(src => src.TripInformation.FloatingUnit.NameAr))
                 .ForMember(des => des.FloatingUnitNameEn, opt => opt.MapFrom(src => src.TripInformation.FloatingUnit.NameEn))
                 .ForMember(des => des.TripInformationCode, opt => opt.MapFrom(src => src.TripInformation.Code)).ReverseMap();
            CreateMap<TripMarina, EditTripMarinaDto>().ReverseMap();
            CreateMap<AddTripMarinaDto, TripMarina>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}

