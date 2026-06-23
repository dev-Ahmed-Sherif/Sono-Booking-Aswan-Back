using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.AllowedDayBeforeReservation;
using SonoBooking.Common.DTO.Lookup.AllowedDayBeforeReservation.Parameters;
using SonoBooking.Domain.Entities.Lookups;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.AllowedDayBeforeReservation
{
    public interface IAllowedDayBeforeReservationService : IBaseService<Entities.Lookups.AllowedDayBeforeReservation, AddAllowedDayBeforeReservationDto, EditAllowedDayBeforeReservationDto, AllowedDayBeforeReservationDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<AllowedDayBeforeReservationFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
