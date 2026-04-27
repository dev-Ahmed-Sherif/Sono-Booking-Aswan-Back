using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Common.Core;

namespace SonoTracker.Application.Services.Enums.Genders
{
    public interface IGenderService
    {
        Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
