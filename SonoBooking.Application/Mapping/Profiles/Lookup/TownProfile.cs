using SonoBooking.Common.DTO.Lookup.Town;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapTown()
        {
            CreateMap<Town, TownDto>()
                .ForMember(dest => dest.City, cfg => cfg.MapFrom(src => src.City.NameAr))
                .ForMember(dest => dest.Governorate, cfg => cfg.MapFrom(src => src.City.Governorate.NameAr))
                .ReverseMap();

            CreateMap<Town, EditTownDto>().ReverseMap();

            CreateMap<AddTownDto, Town>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}

