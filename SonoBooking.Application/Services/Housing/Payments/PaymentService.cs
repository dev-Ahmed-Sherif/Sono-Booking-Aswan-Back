using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Payment;
using SonoBooking.Common.DTO.Housing.Payment.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System;
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

        public override async Task<IFinalResult> AddAsync(AddPaymentDto model, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(model.ReservationId)
                && await UnitOfWork.Repository.Any(
                    p => p.ReservationId == model.ReservationId.Trim() && !p.IsDeleted,
                    cancellationToken))
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.Conflict, exception: null,
                    message: MessagesConstants.Existed);
            }

            Payment entity = Mapper.Map<AddPaymentDto, Payment>(model);

            SetEntityCreatedBaseProperties(entity);

            await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

            int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

            if (affectedRows <= 0)
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: MessagesConstants.AddError);

            return ResponseResult.PostResult(result: entity.Id, status: HttpStatusCode.Created, exception: null,
                message: MessagesConstants.AddSuccess);
        }

        public override async Task<IFinalResult> UpdateAsync(AddPaymentDto model, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(model.ReservationId)
                && await UnitOfWork.Repository.Any(
                    p => p.ReservationId == model.ReservationId.Trim()
                        && !p.IsDeleted
                        && p.Id != model.Id,
                    cancellationToken))
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                    message: MessagesConstants.Existed);
            }

            return await base.UpdateAsync(model, cancellationToken);
        }
    }
}
