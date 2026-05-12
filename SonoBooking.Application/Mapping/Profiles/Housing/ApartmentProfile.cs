using SonoBooking.Common.DTO.Housing.Apartment;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapApartment()
        {
            CreateMap<Apartment, ApartmentDto>()
                .ForMember(des => des.City,src => src.MapFrom(s => s.City.NameAr))
                .ForMember(des => des.Governorate,src => src.MapFrom(s => s.Governorate.NameAr))
                .ForMember(des => des.ApartmentType,src => src.MapFrom(s => s.ApartmentType.NameAr))
                .ReverseMap();

            CreateMap<Apartment, EditApartmentDto>().ReverseMap();

            CreateMap<AddApartmentDto, Apartment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
