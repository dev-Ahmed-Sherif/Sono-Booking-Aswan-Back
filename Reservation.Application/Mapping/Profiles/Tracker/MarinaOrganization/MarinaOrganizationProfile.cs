using SonoTracker.Common.DTO.Reports.TouristMarina;
using SonoTracker.Common.DTO.Tracker.TouristMarina;
using SonoTracker.Common.DTO.Tracker.TouristMarinaOrganization;
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
        public void MapMarinaOrganization()
        {
            CreateMap<TouristMarinaOrganization, TouristMarinaOrganizationDto>().ReverseMap();
            CreateMap<AddTouristMarinaOrganizationDto, TouristMarinaOrganization>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<TouristMarinaOrganization, EditTouristMarinaOrganizationDto>().ReverseMap();
            CreateMap<TouristMarinaOrganization, TouristMarinaReportDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.TouristMarina.NameAr))
                .ForMember(dest => dest.NameOwner, opt => opt.MapFrom(src => src.Organization.NameAr))
                .ForMember(dest => dest.Length, opt => opt.MapFrom(src => src.TouristMarina.Length))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.TouristMarina.MarinaAddress))
                .ForMember(dest => dest.Town, opt => opt.MapFrom(src => src.TouristMarina.City.NameAr))
                .ForMember(dest => dest.North, opt => opt.MapFrom(src => src.TouristMarina.GeoPoint.North))
                .ForMember(dest => dest.East, opt => opt.MapFrom(src => src.TouristMarina.GeoPoint.East))
                .ReverseMap();
        }
    }
}
