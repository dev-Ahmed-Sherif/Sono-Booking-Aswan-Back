using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Payment;
using SonoBooking.Common.DTO.Housing.Payment.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Payments
{
    public interface IPaymentService : IBaseService<Payment, AddPaymentDto, EditPaymentDto, PaymentDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<PaymentFilter> filter, CancellationToken cancellationToken = default);
    }
}
