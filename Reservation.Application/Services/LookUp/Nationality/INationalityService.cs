using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using System;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Lookup.Nationality;
using SonoTracker.Common.DTO.Lookup.Nationality.Parameters;

namespace SonoTracker.Application.Services.Lookup.Nationality
{
    public interface INationalityService : IBaseService<Domain.Entities.Lookups.Nationality, AddNationalityDto, EditNationalityDto, NationalityDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<NationalityFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
