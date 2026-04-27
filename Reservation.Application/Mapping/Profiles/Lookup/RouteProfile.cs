
using SonoTracker.Common.DTO.Lookup.AccidentType;
using SonoTracker.Common.DTO.Lookup.Route;
using SonoTracker.Domain.Entities.Lookups;


namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapRoute()
        {
            CreateMap<Route, RouteDto>().ReverseMap();

            CreateMap<Route, EditRouteDto>().ReverseMap();

            CreateMap<AddRouteDto, Route>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}