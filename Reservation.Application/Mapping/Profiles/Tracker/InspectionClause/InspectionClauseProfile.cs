using SonoTracker.Common.DTO.Tracker.InspectionClause;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapInspectionClause()
        {
            CreateMap<InspectionClause, InspectionClauseDto>()
                .ForMember(des => des.InspectionTypeName, opt => opt.MapFrom(src => src.InspectionType.NameAr))
                .ForMember(des => des.Parent, opt => opt.MapFrom(src => src.Parent.Name))
                .ReverseMap();
            CreateMap<InspectionClause, EditInspectionClauseDto>()
                .ForMember(des => des.InspectionTypeName, opt => opt.MapFrom(src => src.InspectionType.NameAr))
                .ForMember(des => des.ParentName, opt => opt.MapFrom(src => src.Parent.Name))
                .ReverseMap();
            CreateMap<AddInspectionClauseDto, InspectionClause>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
