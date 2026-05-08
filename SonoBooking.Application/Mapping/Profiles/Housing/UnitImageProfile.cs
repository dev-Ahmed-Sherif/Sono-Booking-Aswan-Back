using SonoBooking.Common.DTO.Housing.UnitImage;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapUnitImage()
        {
            CreateMap<UnitImage, UnitImageDto>().ReverseMap();

            CreateMap<UnitImage, EditUnitImageDto>().ReverseMap();

            CreateMap<AddUnitImageDto, UnitImage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
