using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Enums.ReservationStatuses
{
    public class ReservationStatusService : IReservationStatusService
    {
        public async Task<IFinalResult> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var enumNames = Enum.GetValues(typeof(ReservationStatus)).Cast<ReservationStatus>().Select(e => e.GetName()).ToList();
            return await Task.FromResult(new ResponseResult().PostResult(enumNames, HttpStatusCode.OK));
        }
    }
}
