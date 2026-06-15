using Microsoft.EntityFrameworkCore;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.RequestAttach;
using SonoBooking.Common.DTO.Housing.RequestAttach.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.RequestAttaches
{
    public class RequestAttachService(IServiceBaseParameter<RequestAttach> businessBaseParameter)
        : BaseService<RequestAttach, AddRequestAttachDto, EditRequestAttachDto, RequestAttachDto, string, string>(businessBaseParameter),
            IRequestAttachService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<RequestAttachFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            RequestAttachFilter requestAttachFilter = filter?.Filter ?? new RequestAttachFilter();

            (int Count, IEnumerable<RequestAttach> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x =>
                    x.IsDeleted == requestAttachFilter.IsDeleted &&
                    (string.IsNullOrWhiteSpace(requestAttachFilter.RequestId) || x.RequestId == requestAttachFilter.RequestId),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                include: src => src.Include(a => a.Attachment),
                cancellationToken: cancellationToken);

            IEnumerable<RequestAttachDto> data = Mapper.Map<IEnumerable<RequestAttach>, IEnumerable<RequestAttachDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            RequestAttach entity = await UnitOfWork.Repository.FirstOrDefaultAsync(
                x => x.Id.Equals(id),
                include: src => src.Include(a => a.Attachment),
                disableTracking: true,
                cancellationToken: cancellationToken);

            if (entity == null)
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.NotFound, exception: null,
                    message: MessagesConstants.NotFound);

            RequestAttachDto data = Mapper.Map<RequestAttach, RequestAttachDto>(entity);
            return ResponseResult.PostResult(result: data, status: HttpStatusCode.OK, exception: null,
                message: MessagesConstants.Success);
        }
    }
}
