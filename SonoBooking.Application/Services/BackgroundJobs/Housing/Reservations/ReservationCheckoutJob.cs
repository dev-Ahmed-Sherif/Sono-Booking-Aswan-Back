using Hangfire;
using SonoBooking.Common.Infrastructure.UnitOfWork;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.BackgroundJobs.Housing.Reservations
{
    public class ReservationCheckoutJob(IUnitOfWork<Reservation> unitOfWork)
    {
        private const string SystemActor = "System";

        public static void RegisterDailySchedule()
        {
            RecurringJob.AddOrUpdate<ReservationCheckoutJob>(
                "reservation-checkout",
                job => job.ReservationCheckoutJobAsync(),
                Cron.Daily(7));
        }

        public async Task ReservationCheckoutJobAsync()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            IEnumerable<Reservation> reservations = await unitOfWork.Repository.FindAsync(
                r => !r.IsDeleted
                    && r.Status == ReservationStatus.Completed
                    && r.EndDate == today,
                disableTracking: false);

            foreach (Reservation reservation in reservations)
            {
                if (reservation.Status != ReservationStatus.Completed)
                    continue;

                reservation.Status = ReservationStatus.Checkout;
                TouchAudit(reservation);
            }

            await unitOfWork.SaveChangesAsync();
        }

        private static void TouchAudit(BaseAudit<string> entity)
        {
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = SystemActor;
            entity.ModifiedById = SystemActor;
        }
    }
}
