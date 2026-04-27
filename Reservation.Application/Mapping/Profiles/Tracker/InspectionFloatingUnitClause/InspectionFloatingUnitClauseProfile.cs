using SonoTracker.Common.DTO.Tracker.InspectionFloatingUnitClause;
using SonoTracker.Domain.Entities.Tracker;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapInspectionFloatingUnitClause()
        {
            CreateMap<InspectionFloatingUnitClause, InspectionFloatingUnitClauseDto>()
                .ForMember(des => des.InspectionClauseName, opt => opt.MapFrom(src => src.InspectionClause.Name))
                .ForMember(des => des.InspectionClauseCode, opt => opt.MapFrom(src => src.InspectionClause.Code))
                .ForMember(des => des.InspectionDate, opt => opt.MapFrom(src => src.Inspection.InspectionDate))
                .ReverseMap();
            CreateMap<InspectionFloatingUnitClause, EditInspectionFloatingUnitClauseDto>()
                .ForMember(des => des.InspectionClauseName, opt => opt.MapFrom(src => src.InspectionClause.Name))
                .ForMember(des => des.InspectionDate, opt => opt.MapFrom(src => src.Inspection.InspectionDate))
                .ReverseMap();
            CreateMap<AddInspectionFloatingUnitClauseDto, InspectionFloatingUnitClause>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
