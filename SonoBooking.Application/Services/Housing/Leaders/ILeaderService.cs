using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Leader;
using SonoBooking.Common.DTO.Housing.Leader.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Leaders
{
    public interface ILeaderService : IBaseService<Leader, AddLeaderDto, EditLeaderDto, LeaderDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<LeaderFilter> filter, CancellationToken cancellationToken = default);
    }
}
