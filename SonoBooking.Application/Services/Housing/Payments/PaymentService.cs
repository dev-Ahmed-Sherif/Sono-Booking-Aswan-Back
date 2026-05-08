using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Payment;
using SonoBooking.Common.DTO.Housing.Payment.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Payments
{
    public class PaymentService(IServiceBaseParameter<Payment> businessBaseParameter) : BaseService<Payment, AddPaymentDto, EditPaymentDto, PaymentDto, string, string>(businessBaseParameter), IPaymentService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<PaymentFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            PaymentFilter paymentFilter = filter?.Filter ?? new PaymentFilter();

            (int Count, IEnumerable<Payment> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == paymentFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<PaymentDto> data = Mapper.Map<IEnumerable<Payment>, IEnumerable<PaymentDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
