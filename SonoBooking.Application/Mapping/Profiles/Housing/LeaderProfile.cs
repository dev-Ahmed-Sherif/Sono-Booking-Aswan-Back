using SonoBooking.Common.DTO.Housing.Leader;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapLeader()
        {
            CreateMap<Leader, LeaderDto>()
                .ForMember(dest => dest.HasFileContent, opt => opt.MapFrom(src => src.FileContent != null && src.FileContent.Length > 0))
                .ForMember(dest => dest.FileContentBase64, opt => opt.Ignore());

            CreateMap<Leader, EditLeaderDto>()
                .ForMember(dest => dest.File, opt => opt.Ignore());

            CreateMap<AddLeaderDto, Leader>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FileContent, opt => opt.Ignore());
        }
    }
}
