using SonoBooking.Common.DTO.Housing.Request;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapRequest()
        {
            CreateMap<Request, RequestDto>().ReverseMap();

            CreateMap<Request, EditRequestDto>().ReverseMap();

            CreateMap<AddRequestDto, Request>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
