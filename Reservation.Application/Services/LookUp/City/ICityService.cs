using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.City;
using SonoTracker.Common.DTO.Lookup.City.Parameters;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Lookup.City
{
    public interface ICityService : IBaseService<Domain.Entities.Lookups.City, AddCityDto, EditCityDto, CityDto, string, string>
    {
    Task<IFinalResult> GetAllAsync(string governorateId, CancellationToken cancellationToken = default);

    Task<PagingResult> GetAllPagedAsync(BaseParam<CityFilter> filter, CancellationToken cancellationToken = default);

    Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
