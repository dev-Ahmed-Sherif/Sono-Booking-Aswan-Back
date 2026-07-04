using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Availability;

public interface IUnitAvailabilityGapService
{
    /// <summary>
    /// Opens catalog status for units in the gap after an early checkout when a later approved stay exists.
    /// </summary>
    Task RefreshGapAvailabilityForReservationAsync(
        string reservationId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Scans all reservations with checkout set and refreshes open gap windows.
    /// </summary>
    Task RefreshAllOpenGapsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs gap refresh now when the window is open, or schedules it at checkout blocking end.
    /// </summary>
    Task OnActualCheckOutDateChangedAsync(
        string reservationId,
        CancellationToken cancellationToken = default);
}
