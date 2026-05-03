using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.Town;
using SonoTracker.Common.DTO.Lookup.Town.Parameters;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Lookup.Town
{
    public interface ITownService : IBaseService<SonoBooking.Domain.Entities.Lookups.Town, AddTownDto, EditTownDto, TownDto, string, string>
    {
    Task<PagingResult> GetAllPagedAsync(BaseParam<TownFilter> filter, CancellationToken cancellationToken = default);

    Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
