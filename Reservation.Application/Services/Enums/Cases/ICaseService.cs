using SonoTracker.Common.Core;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Enums.Cases
{
    public interface ICaseService
    {
        Task<IFinalResult> GetAllAsync(System.Threading.CancellationToken cancellationToken = default);
    }
}
