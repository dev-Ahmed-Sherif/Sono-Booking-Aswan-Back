using SonoBooking.Common.DTO.Housing.RequestUnit;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapRequestUnit()
        {
            CreateMap<RequestUnit, RequestUnitDto>().ReverseMap();

            CreateMap<RequestUnit, EditRequestUnitDto>().ReverseMap();

            CreateMap<AddRequestUnitDto, RequestUnit>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
