using SonoBooking.Common.DTO.Housing.Apartment;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapApartment()
        {
            CreateMap<Apartment, ApartmentDto>().ReverseMap();

            CreateMap<Apartment, EditApartmentDto>().ReverseMap();

            CreateMap<AddApartmentDto, Apartment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
