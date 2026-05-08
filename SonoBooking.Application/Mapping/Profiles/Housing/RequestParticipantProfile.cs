using SonoBooking.Common.DTO.Housing.RequestParticipant;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapRequestParticipant()
        {
            CreateMap<RequestParticipant, RequestParticipantDto>().ReverseMap();

            CreateMap<RequestParticipant, EditRequestParticipantDto>().ReverseMap();

            CreateMap<AddRequestParticipantDto, RequestParticipant>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
