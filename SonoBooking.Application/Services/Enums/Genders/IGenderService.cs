using SonoBooking.Common.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Enums.Genders
{
    public interface IGenderService
    {
        Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default);
    }
}

