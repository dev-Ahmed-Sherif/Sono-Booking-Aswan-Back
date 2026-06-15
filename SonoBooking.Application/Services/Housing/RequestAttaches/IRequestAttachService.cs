using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.RequestAttach;
using SonoBooking.Common.DTO.Housing.RequestAttach.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.RequestAttaches
{
    public interface IRequestAttachService : IBaseService<RequestAttach, AddRequestAttachDto, EditRequestAttachDto, RequestAttachDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<RequestAttachFilter> filter, CancellationToken cancellationToken = default);
    }
}
