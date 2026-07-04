using Hangfire;
using SonoBooking.Application.Services.Housing.Availability;
using System;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.BackgroundJobs.Housing.Reservations;

public class ReservationUnitAvailabilityGapJob(IUnitAvailabilityGapService gapService)
{
    public static void RegisterSchedules()
    {
        RecurringJob.AddOrUpdate<ReservationUnitAvailabilityGapJob>(
            "reservation-unit-availability-gap",
            job => job.RefreshAllOpenGapsAsync(),
            Cron.Hourly());
    }

    public static void EnqueueForReservation(string reservationId) =>
        BackgroundJob.Enqueue<ReservationUnitAvailabilityGapJob>(
            job => job.ProcessReservationGapAsync(reservationId));

    public static void ScheduleForReservation(string reservationId, DateTime runAt)
    {
        TimeSpan delay = runAt - DateTime.Now;
        if (delay <= TimeSpan.Zero)
        {
            EnqueueForReservation(reservationId);
            return;
        }

        BackgroundJob.Schedule<ReservationUnitAvailabilityGapJob>(
            job => job.ProcessReservationGapAsync(reservationId),
            delay);
    }

    public Task ProcessReservationGapAsync(string reservationId) =>
        gapService.RefreshGapAvailabilityForReservationAsync(reservationId);

    public Task RefreshAllOpenGapsAsync() =>
        gapService.RefreshAllOpenGapsAsync();
}
