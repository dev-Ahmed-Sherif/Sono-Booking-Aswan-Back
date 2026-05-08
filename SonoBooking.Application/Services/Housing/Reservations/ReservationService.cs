using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Reservation;
using SonoBooking.Common.DTO.Housing.Reservation.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Reservations
{
    public class ReservationService(IServiceBaseParameter<Reservation> businessBaseParameter) : BaseService<Reservation, AddReservationDto, EditReservationDto, ReservationDto, string, string>(businessBaseParameter), IReservationService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<ReservationFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            ReservationFilter reservationFilter = filter?.Filter ?? new ReservationFilter();

            (int Count, IEnumerable<Reservation> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == reservationFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<ReservationDto> data = Mapper.Map<IEnumerable<Reservation>, IEnumerable<ReservationDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
