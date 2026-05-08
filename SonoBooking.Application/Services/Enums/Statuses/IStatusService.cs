using SonoBooking.Common.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Enums.Statuses
{
    public interface IStatusService
    {
        Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
