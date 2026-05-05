using SonoBooking.Domain.Entities.Lookups;
using SonoTracker.Common.DTO.Lookup.City;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapCity()
        {
            CreateMap<City, CityDto>().ReverseMap();
            
            CreateMap<City, EditCityDto>().ReverseMap();

            CreateMap<AddCityDto, City>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
