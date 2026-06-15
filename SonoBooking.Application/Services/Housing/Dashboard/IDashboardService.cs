using SonoBooking.Common.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Dashboard;

public interface IDashboardService
{
    Task<IFinalResult> GetGovernorSummaryAsync(CancellationToken cancellationToken = default);
}
