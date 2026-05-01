using SonoTracker.Common.DTO.Lookup.OrganizationCategory;
using SonoTracker.Domain.Entities.Lookups;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapOrganizationCategory()
        {
            CreateMap<OrganizationCategory, OrganizationCategoryDto>().ReverseMap();

            CreateMap<OrganizationCategory, EditOrganizationCategoryDto>().ReverseMap();

            CreateMap<AddOrganizationCategoryDto, OrganizationCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
