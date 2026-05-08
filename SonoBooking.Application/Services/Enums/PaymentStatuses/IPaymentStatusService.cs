using SonoBooking.Common.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Enums.PaymentStatuses
{
    public interface IPaymentStatusService
    {
        Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
