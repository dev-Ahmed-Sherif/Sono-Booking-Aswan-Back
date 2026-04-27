using SonoTracker.Common.DTO.Tracker.Accident;
using SonoTracker.Domain.Entities.Tracker;
using System;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapAccident()
        {
            CreateMap<Accident, AccidentDto>()
                .ForMember(des => des.AccidentDate, opt => opt.MapFrom(src => src.AccidentDate.ToDateTime(TimeOnly.MinValue)))
                .ForMember(des => des.ResponseDate, opt => opt.MapFrom(src => src.ResponseDate.HasValue ? src.ResponseDate.Value.ToDateTime(TimeOnly.MinValue) : default))
                .ForMember(des => des.AccidentType, opt => opt.MapFrom(src => src.AccidentType.NameAr))
                .ForMember(des => des.FloatingUnit, opt => opt.MapFrom(src => src.FloatingUnit.NameAr))
                .ForMember(des => des.Town, opt => opt.MapFrom(src => src.City.NameAr));

            CreateMap<Accident, EditAccidentDto>()
                .ForMember(des => des.AccidentDate, opt => opt.MapFrom(src => src.AccidentDate.ToDateTime(TimeOnly.MinValue)))
                .ForMember(des => des.ResponseDate, opt => opt.MapFrom(src => src.ResponseDate.HasValue ? src.ResponseDate.Value.ToDateTime(TimeOnly.MinValue) : default))
                .ForMember(des => des.AccidentType, opt => opt.MapFrom(src => src.AccidentType.NameAr))
                .ForMember(des => des.FloatingUnit, opt => opt.MapFrom(src => src.FloatingUnit.NameAr))
                .ForMember(des => des.City, opt => opt.MapFrom(src => src.City.NameAr));

            CreateMap<AddAccidentDto, Accident>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ForMember(des => des.Attach, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
