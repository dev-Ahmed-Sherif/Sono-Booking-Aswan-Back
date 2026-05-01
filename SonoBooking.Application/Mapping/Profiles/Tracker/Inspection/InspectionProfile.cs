using SonoTracker.Common.DTO.Tracker.GeneralInspection;
using SonoTracker.Common.DTO.Tracker.Inspection;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapInspection()
        {

            // Properly-named Inspection DTOs
            CreateMap<Inspection, InspectionDto>()
                .ForMember(des => des.Organization, opt => opt.MapFrom(src => src.Organization.NameAr))
                //.ForMember(des => des.OrganizationNameEn, opt => opt.MapFrom(src => src.Organization.NameEn))
                .ForMember(des => des.FloatingUnit, opt => opt.MapFrom(src => src.FloatingUnit.NameAr))
                //.ForMember(des => des.FloatingUnitNameEn, opt => opt.MapFrom(src => src.FloatingUnit.NameEn))
                .ReverseMap();
            CreateMap<Inspection, EditInspectionDto>()
                .ForMember(des => des.OrganizationNameAr, opt => opt.MapFrom(src => src.Organization.NameAr))
                .ForMember(des => des.OrganizationNameEn, opt => opt.MapFrom(src => src.Organization.NameEn))
                .ForMember(des => des.FloatingUnitNameAr, opt => opt.MapFrom(src => src.FloatingUnit.NameAr))
                .ForMember(des => des.FloatingUnitNameEn, opt => opt.MapFrom(src => src.FloatingUnit.NameEn))
                .ReverseMap();
            CreateMap<AddInspectionDto, Inspection>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ForMember(des => des.InspectionFloatingUnitClauses, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
