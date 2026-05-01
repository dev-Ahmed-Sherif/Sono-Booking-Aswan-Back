using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.Governorate.Parameters;
using SonoTracker.Common.DTO.Tracker.Governorate;
using SonoTracker.Domain.Entities.Lookups;

namespace SonoTracker.Application.Services.Tracker.Governorates
{
    public interface IGovernorateService : IBaseService<Governorate, AddGovernorateDto, EditGovernorateDto, GovernorateDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<GovernorateFilter> filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
