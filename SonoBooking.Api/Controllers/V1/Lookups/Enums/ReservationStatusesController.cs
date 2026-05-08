using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonoBooking.Api.Controllers.V1.Base;
using SonoBooking.Application.Services.Enums.ReservationStatuses;
using SonoBooking.Common.Core;
using System.Threading;

namespace SonoBooking.Api.Controllers.V1.Lookups.Enums
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class ReservationStatusesController(IReservationStatusService reservationStatusService) : BaseController
    {
        [HttpGet("getAll")]
        public async Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default)
            => await reservationStatusService.GetAllAsync(cancellationToken);
    }
}
