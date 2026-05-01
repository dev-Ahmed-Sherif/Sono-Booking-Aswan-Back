using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.TripGeo;
using SonoTracker.Common.DTO.Tracker.TripGeo.Parameters;

namespace SonoTracker.Application.Services.Tracker.TripGeo
{
    public interface ITripGeoService : IBaseService<Entities.Tracker.TripGeo, AddTripGeoDto, EditTripGeoDto, TripGeoDto, string, string>
    {
        Task<IFinalResult> GetLastByFloatingUnitIdAsync(object id, CancellationToken cancellationToken = default);
        Task<PagingResult> GetAllPagedAsync(BaseParam<TripGeoFilter> filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetAllFilterAsync(TripGeoFilter filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
