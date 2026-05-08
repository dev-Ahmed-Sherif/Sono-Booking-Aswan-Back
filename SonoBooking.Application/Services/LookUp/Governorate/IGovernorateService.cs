using SonoBooking.Application.Services.Base;
using SonoBooking.Common.DTO.Lookup.Governorate;
using SonoBooking.Common.DTO.Lookup.Governorate.Parameters;
using GovernorateEntity = SonoBooking.Domain.Entities.Lookups.Governorate;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.Governorate
{
    public interface IGovernorateService : IBaseService<GovernorateEntity, AddGovernorateDto, EditGovernorateDto, GovernorateDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<GovernorateFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
