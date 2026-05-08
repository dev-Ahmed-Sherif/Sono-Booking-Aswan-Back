using SonoBooking.Common.DTO.Lookup.ApartmentType;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapApartmentType()
        {
            CreateMap<ApartmentType, ApartmentTypeDto>().ReverseMap();

            CreateMap<ApartmentType, EditApartmentTypeDto>().ReverseMap();

            CreateMap<AddApartmentTypeDto, ApartmentType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
