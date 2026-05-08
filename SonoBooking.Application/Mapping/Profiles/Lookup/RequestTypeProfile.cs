using SonoBooking.Common.DTO.Lookup.RequestType;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapRequestType()
        {
            CreateMap<RequestType, RequestTypeDto>().ReverseMap();

            CreateMap<RequestType, EditRequestTypeDto>().ReverseMap();

            CreateMap<AddRequestTypeDto, RequestType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
