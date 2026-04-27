using SonoTracker.Common.DTO.Tracker.FloatingUnitOrganization;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapFloatingUnitOrganization()
        {
            CreateMap<FloatingUnitOrganization, FloatingUnitOrganizationDto>()
            .ForMember(des => des.OrganizationNameAr, opt => opt.MapFrom(src => src.Organization.NameAr))
            .ForMember(des => des.OrganizationNameEn, opt => opt.MapFrom(src => src.Organization.NameEn))
            .ForMember(des => des.FloatingUnitNameAr, opt => opt.MapFrom(src => src.FloatingUnit.NameAr))
            .ForMember(des => des.FloatingUnitNameEn, opt => opt.MapFrom(src => src.FloatingUnit.NameEn))
            .ForMember(des => des.OrganizationType, opt => opt.MapFrom(src => src.Organization.OrganizationType.GetName()))
            .ReverseMap();

            CreateMap<FloatingUnitOrganization, EditFloatingUnitOrganizationDto>().ReverseMap();

            CreateMap<AddFloatingUnitOrganizationDto, FloatingUnitOrganization>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
