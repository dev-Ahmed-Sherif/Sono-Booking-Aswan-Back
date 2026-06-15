using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Reservation;
using SonoBooking.Common.DTO.Housing.Reservation.Parameters;
using SonoBooking.Common.DTO.Reports.Reservations;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Reservations
{
    public interface IReservationService : IBaseService<Reservation, AddReservationDto, EditReservationDto, ReservationDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<ReservationFilter> filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetAllReportAsync(FilterReservationReportDto filter, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateReportAsync(FilterReservationReportDto filter, CancellationToken cancellationToken = default);
    }
}
