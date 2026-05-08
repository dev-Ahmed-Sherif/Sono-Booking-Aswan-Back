using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Request;
using SonoBooking.Common.DTO.Housing.Request.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Requests
{
    public interface IRequestService : IBaseService<Request, AddRequestDto, EditRequestDto, RequestDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<RequestFilter> filter, CancellationToken cancellationToken = default);
    }
}
