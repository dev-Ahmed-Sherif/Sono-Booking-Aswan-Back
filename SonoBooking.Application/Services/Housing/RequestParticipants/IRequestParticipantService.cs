using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.RequestParticipant;
using SonoBooking.Common.DTO.Housing.RequestParticipant.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.RequestParticipants
{
    public interface IRequestParticipantService : IBaseService<RequestParticipant, AddRequestParticipantDto, EditRequestParticipantDto, RequestParticipantDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<RequestParticipantFilter> filter, CancellationToken cancellationToken = default);
    }
}
