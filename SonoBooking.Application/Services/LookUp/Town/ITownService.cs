using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.Town;
using SonoBooking.Common.DTO.Lookup.Town.Parameters;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Lookup.Town
{
    public interface ITownService : IBaseService<SonoBooking.Domain.Entities.Lookups.Town, AddTownDto, EditTownDto, TownDto, string, string>
    {
    Task<PagingResult> GetAllPagedAsync(BaseParam<TownFilter> filter, CancellationToken cancellationToken = default);

    Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}

