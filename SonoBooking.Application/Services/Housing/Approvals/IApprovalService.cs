using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Approval;
using SonoBooking.Common.DTO.Housing.Approval.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Approvals
{
    public interface IApprovalService : IBaseService<Approval, AddApprovalDto, EditApprovalDto, ApprovalDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<ApprovalFilter> filter, CancellationToken cancellationToken = default);
    }
}
