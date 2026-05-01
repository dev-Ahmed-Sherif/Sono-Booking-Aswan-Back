using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.TripMarina;
using SonoTracker.Common.DTO.Tracker.TripMarina.Parameters;


namespace SonoTracker.Application.Services.Tracker.TripMarina
{
    public interface ITripMarinaService : IBaseService<Entities.Tracker.TripMarina, AddTripMarinaDto, EditTripMarinaDto, TripMarinaDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<TripMarinaFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}