using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Approval;
using SonoBooking.Common.DTO.Housing.Approval.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Approvals
{
    public class ApprovalService(IServiceBaseParameter<Approval> businessBaseParameter) : BaseService<Approval, AddApprovalDto, EditApprovalDto, ApprovalDto, string, string>(businessBaseParameter), IApprovalService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<ApprovalFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            ApprovalFilter approvalFilter = filter?.Filter ?? new ApprovalFilter();

            (int Count, IEnumerable<Approval> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == approvalFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<ApprovalDto> data = Mapper.Map<IEnumerable<Approval>, IEnumerable<ApprovalDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
