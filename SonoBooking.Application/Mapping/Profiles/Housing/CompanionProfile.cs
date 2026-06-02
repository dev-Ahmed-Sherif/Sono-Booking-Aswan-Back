using SonoBooking.Common.DTO.Housing.Companion;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapCompanion()
        {
            CreateMap<Companion, CompanionDto>().ReverseMap();

            CreateMap<Companion, EditCompanionDto>().ReverseMap();

            CreateMap<AddCompanionDto, Companion>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DocumentImageUrl, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
