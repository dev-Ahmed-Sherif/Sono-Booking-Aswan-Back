using SonoTracker.Common.DTO.Reports.Org;
using SonoTracker.Common.DTO.Tracker.Organization;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapOrganization()
        {
            CreateMap<Organization, OrganizationDto>()
                 //.ForMember(des => des.OrganizationType, opt => opt.MapFrom(src => src.OrganizationType.GetName()))
                 .ForMember(des => des.NationalityName, opt => opt.MapFrom(src => src.Nationality.NameAr))
                 //.ForMember(des => des.InspectionType, opt => opt.MapFrom(src => src.InspectionType.NameAr))
                 .ReverseMap();
            CreateMap<Organization, EditOrganizationDto>()
                //.ForMember(des => des.OrganizationType, opt => opt.MapFrom(src => src.OrganizationType.GetName()))
                 .ForMember(des => des.NationalityName, opt => opt.MapFrom(src => src.Nationality.NameAr))
                 //.ForMember(des => des.InspectionType, opt => opt.MapFrom(src => src.InspectionType.NameAr))
                 //.ForMember(des => des.CommercialRegistrationAttachment, opt => opt.MapFrom(src => src.ImageUrl))
                .ReverseMap();

            CreateMap<AddOrganizationDto, Organization>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Organization, OrgReportDTO>().ReverseMap();
        }
    }
}
