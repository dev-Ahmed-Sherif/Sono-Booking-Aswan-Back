using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Enums.PaymentStatuses
{
    public class PaymentStatusService : IPaymentStatusService
    {
        public async Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var enumNames = Enum.GetValues(typeof(PaymentStatus)).Cast<PaymentStatus>().Select(e => e.GetName()).ToList();
            return await Task.FromResult(new ResponseResult().PostResult(enumNames, HttpStatusCode.OK));
        }
    }
}
