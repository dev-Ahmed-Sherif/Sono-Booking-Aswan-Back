using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.RequestType;
using SonoBooking.Common.DTO.Lookup.RequestType.Parameters;
using SonoBooking.Domain.Entities.Lookups;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.RequestType
{
    public interface IRequestTypeService : IBaseService<Entities.Lookups.RequestType, AddRequestTypeDto, EditRequestTypeDto, RequestTypeDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<RequestTypeFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
