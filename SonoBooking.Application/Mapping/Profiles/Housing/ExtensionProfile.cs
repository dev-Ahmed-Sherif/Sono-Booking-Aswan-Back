using SonoBooking.Common.DTO.Housing.Extension;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapExtension()
        {
            CreateMap<Extension, ExtensionDto>()
                .ForMember(
                    dest => dest.RequestId,
                    opt => opt.MapFrom(src =>
                        src.Reservation != null ? src.Reservation.RequestId : null))
                .ReverseMap();

            CreateMap<Extension, EditExtensionDto>().ReverseMap();

            CreateMap<AddExtensionDto, Extension>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
