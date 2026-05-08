using SonoBooking.Common.DTO.Lookup.Governorate;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapGovernorate()
        {
            CreateMap<Governorate, GovernorateDto>().ReverseMap();

            CreateMap<Governorate, EditGovernorateDto>().ReverseMap();

            CreateMap<AddGovernorateDto, Governorate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                // Keep existing required fields on partial updates.
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Governorate, AddGovernorateDto>();
        }
    }
}
