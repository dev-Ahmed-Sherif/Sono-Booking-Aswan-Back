using SonoBooking.Common.DTO.Housing.Leader;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapLeader()
        {
            CreateMap<Leader, LeaderDto>().ReverseMap();

            CreateMap<Leader, EditLeaderDto>().ReverseMap();

            CreateMap<AddLeaderDto, Leader>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
