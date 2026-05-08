using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.RequestParticipant;
using SonoBooking.Common.DTO.Housing.RequestParticipant.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.RequestParticipants
{
    public class RequestParticipantService(IServiceBaseParameter<RequestParticipant> businessBaseParameter) : BaseService<RequestParticipant, AddRequestParticipantDto, EditRequestParticipantDto, RequestParticipantDto, string, string>(businessBaseParameter), IRequestParticipantService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<RequestParticipantFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            RequestParticipantFilter requestParticipantFilter = filter?.Filter ?? new RequestParticipantFilter();

            (int Count, IEnumerable<RequestParticipant> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == requestParticipantFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<RequestParticipantDto> data = Mapper.Map<IEnumerable<RequestParticipant>, IEnumerable<RequestParticipantDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
