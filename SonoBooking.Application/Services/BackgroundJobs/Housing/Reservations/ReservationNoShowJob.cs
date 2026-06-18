using Hangfire;
using Microsoft.EntityFrameworkCore;
using SonoBooking.Application.Services.Housing.Reservations;
using SonoBooking.Common.Infrastructure.UnitOfWork;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Base;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.BackgroundJobs.Housing.Reservations
{
    public class ReservationNoShowJob(
        IUnitOfWork<Reservation> unitOfWork,
        ReservationStatusEmailNotifier statusEmailNotifier)
    {
        private const string SystemActor = "System";
        private const string NoShowCancelationReason = "تاخر عن معاد تسجيل الوصول";

        public static void RegisterDailySchedule()
        {
            RecurringJob.AddOrUpdate<ReservationNoShowJob>(
                "reservation-no-show",
                job => job.ReservationNoShowJobAsync(),
                Cron.Daily(2));
        }

        public async Task ReservationNoShowJobAsync()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            DateOnly targetStartDate = today.AddDays(-1);

            IEnumerable<Reservation> reservations = await unitOfWork.Repository.FindAsync(
                r => !r.IsDeleted
                    && r.Status == ReservationStatus.Reserved
                    && r.StartDate == targetStartDate,
                include: q => q
                    .Include(r => r.Request)
                        .ThenInclude(req => req.User)
                    .Include(r => r.Request)
                        .ThenInclude(req => req.RequestUnits)
                        .ThenInclude(u => u.Bed)
                    .Include(r => r.Request)
                        .ThenInclude(req => req.RequestUnits)
                        .ThenInclude(u => u.Room)
                    .Include(r => r.Request)
                        .ThenInclude(req => req.RequestUnits)
                        .ThenInclude(u => u.Apartment),
                disableTracking: false);

            List<(Reservation Reservation, ReservationStatus PreviousStatus)> statusChanges = [];

            foreach (Reservation reservation in reservations)
            {
                if (reservation.Status != ReservationStatus.Reserved)
                    continue;

                ReservationStatus previousStatus = reservation.Status;
                reservation.Status = ReservationStatus.NoShow;
                reservation.CancelationReason = NoShowCancelationReason;
                TouchAudit(reservation);
                ReleaseRequestUnits(reservation);
                statusChanges.Add((reservation, previousStatus));
            }

            await unitOfWork.SaveChangesAsync();

            foreach ((Reservation reservation, ReservationStatus previousStatus) in statusChanges)
                await statusEmailNotifier.TrySendStatusChangeEmailAsync(reservation, previousStatus);
        }

        private static void ReleaseRequestUnits(Reservation reservation)
        {
            if (reservation.Request?.RequestUnits == null)
                return;

            foreach (RequestUnit unit in reservation.Request.RequestUnits.Where(u => !u.IsDeleted))
            {
                if (!string.IsNullOrWhiteSpace(unit.BedId) && unit.Bed != null)
                {
                    unit.Bed.Status = UnitStatus.Available;
                    TouchAudit(unit.Bed);
                }

                if (!string.IsNullOrWhiteSpace(unit.RoomId) && unit.Room != null)
                {
                    unit.Room.Status = UnitStatus.Available;
                    TouchAudit(unit.Room);
                }

                if (!string.IsNullOrWhiteSpace(unit.ApartmentId) && unit.Apartment != null)
                {
                    unit.Apartment.Status = UnitStatus.Available;
                    TouchAudit(unit.Apartment);
                }
            }
        }

        private static void TouchAudit(BaseAudit<string> entity)
        {
            entity.ModifiedAt = DateTime.UtcNow;
            entity.ModifiedBy = SystemActor;
            entity.ModifiedById = SystemActor;
        }
    }
}
