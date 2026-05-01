using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.TouristMarina;
using SonoTracker.Common.DTO.Tracker.TouristMarina.Parameters;
using SonoTracker.Common.DTO.Tracker.TripInformation.Parameters;
using SonoTracker.Domain.Entities.Tracker;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.Marinas
{
   public  interface ITouristMarinaService : IBaseService<TouristMarina, AddTouristMarinaDto,EditTouristMarinaDto, TouristMarinaDto, string, string>
   {
        Task<PagingResult> GetAllPagedAsync(BaseParam<TouristMarinaFilter> Filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetAllFilterAsync(TouristMarinaFilter filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
   }
}
