using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Request;
using SonoBooking.Common.DTO.Housing.Request.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Requests
{
    public class RequestService(IServiceBaseParameter<Request> businessBaseParameter) : BaseService<Request, AddRequestDto, EditRequestDto, RequestDto, string, string>(businessBaseParameter), IRequestService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<RequestFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            RequestFilter requestFilter = filter?.Filter ?? new RequestFilter();

            (int Count, IEnumerable<Request> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == requestFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<RequestDto> data = Mapper.Map<IEnumerable<Request>, IEnumerable<RequestDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
