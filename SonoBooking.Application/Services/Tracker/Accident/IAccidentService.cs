using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.Accident;
using SonoTracker.Common.DTO.Tracker.Accident.Parameters;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.Accident
{
   public  interface IAccidentService : IBaseService<Entities.Tracker.Accident, AddAccidentDto, EditAccidentDto, AccidentDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<AccidentFilter> Filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetAllFilterAsync(AccidentFilter filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
