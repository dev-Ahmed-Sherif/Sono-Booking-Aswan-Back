using System;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.Route;
using SonoTracker.Common.DTO.Lookup.Route.Parameters;

namespace SonoTracker.Application.Services.LookUp.Route
{
    public interface IRouteService : IBaseService<Entities.Lookups.Route, AddRouteDto, EditRouteDto, RouteDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<RouteFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);

    }
}
