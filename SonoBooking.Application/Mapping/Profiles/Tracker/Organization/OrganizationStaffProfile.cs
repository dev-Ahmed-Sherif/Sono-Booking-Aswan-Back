using SonoTracker.Common.DTO.Tracker.Organization;
using SonoTracker.Common.DTO.Tracker.OrganizationStaff;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapOrganizationStaff()
        {
            CreateMap<OrganizationStaff, OrganizationStaffDto>()
                 .ForMember(des => des.OrganizationName, opt => opt.MapFrom(src => src.Organization.NameAr))
                 .ReverseMap();

            CreateMap<OrganizationStaff, EditOrganizationStaffDto>().ReverseMap();

            CreateMap<AddOrganizationStaffDto, OrganizationStaff>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
