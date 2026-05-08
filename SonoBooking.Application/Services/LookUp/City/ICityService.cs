using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.City;
using SonoBooking.Common.DTO.Lookup.City.Parameters;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Lookup.City
{
    public interface ICityService : IBaseService<Domain.Entities.Lookups.City, AddCityDto, EditCityDto, CityDto, string, string>
    {
    Task<IFinalResult> GetAllAsync(string governorateId, CancellationToken cancellationToken = default);

    Task<PagingResult> GetAllPagedAsync(BaseParam<CityFilter> filter, CancellationToken cancellationToken = default);

    Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}

