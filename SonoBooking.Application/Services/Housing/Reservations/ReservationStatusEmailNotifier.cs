using SonoBooking.Application.Services.Email;
using SonoBooking.Common.Infrastructure.UnitOfWork;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using SonoBooking.Domain.Entities.Identity;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Reservations
{
    public class ReservationStatusEmailNotifier(
        IUnitOfWork<Reservation> unitOfWork,
        IEmailService emailService)
    {
        public async Task TrySendStatusChangeEmailAsync(
            Reservation reservation,
            ReservationStatus previousStatus,
            CancellationToken cancellationToken = default)
        {
            if (!ShouldNotify(previousStatus, reservation.Status))
                return;

            Request request = reservation.Request;
            if (request == null)
            {
                request = await unitOfWork.GetRepository<Request>().FirstOrDefaultAsync(
                    x => x.Id == reservation.RequestId,
                    disableTracking: true,
                    cancellationToken: cancellationToken);
            }

            if (request == null)
                return;

            User owner = request.User;
            if (owner == null)
            {
                owner = await unitOfWork.GetRepository<User>().FirstOrDefaultAsync(
                    u => u.Id == request.UserId,
                    disableTracking: true,
                    cancellationToken: cancellationToken);
            }

            if (owner == null || string.IsNullOrWhiteSpace(owner.Email))
                return;

            try
            {
                string subject = BuildSubject(previousStatus, reservation.Status);
                string statusMessage = BuildStatusMessage(previousStatus, reservation.Status, reservation);
                string body = $"""
                    <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً {WebUtility.HtmlEncode(owner.FullName)}</h2>
                    <p>تم تحديث حالة الحجز لطلب رقم <strong>{WebUtility.HtmlEncode(request.RequestNumber)}</strong>.</p>
                    <p><strong>{WebUtility.HtmlEncode(statusMessage)}</strong></p>
                    <p><strong>تاريخ الوصول:</strong> {reservation.StartDate:dd/MM/yyyy}</p>
                    <p><strong>تاريخ المغادرة:</strong> {reservation.EndDate:dd/MM/yyyy}</p>
                    </div>
                    """;

                await emailService.SendEmailAsync(owner.Email, subject, body);
            }
            catch
            {
                // Status change is already persisted; do not fail when email delivery fails.
            }
        }

        private static bool ShouldNotify(ReservationStatus previousStatus, ReservationStatus currentStatus)
        {
            if (previousStatus == currentStatus)
                return false;

            return (previousStatus, currentStatus) switch
            {
                (ReservationStatus.Reserved, ReservationStatus.Completed) => true,
                (ReservationStatus.Reserved, ReservationStatus.NoShow) => true,
                (ReservationStatus.NoShow, ReservationStatus.Reserved) => true,
                (ReservationStatus.Reserved, ReservationStatus.Canceled) => true,
                (ReservationStatus.Canceled, ReservationStatus.Reserved) => true,
                _ => false
            };
        }

        private static string BuildSubject(ReservationStatus previousStatus, ReservationStatus currentStatus)
        {
            if (previousStatus == ReservationStatus.Reserved && currentStatus == ReservationStatus.Completed)
                return "تأكيد الإقامة - نظام حجز الإسكان";

            if (currentStatus == ReservationStatus.Reserved
                && previousStatus is ReservationStatus.NoShow or ReservationStatus.Canceled)
                return "استعادة الحجز - نظام حجز الإسكان";

            return "تحديث حالة الحجز - نظام حجز الإسكان";
        }

        private static string BuildStatusMessage(
            ReservationStatus previousStatus,
            ReservationStatus currentStatus,
            Reservation reservation)
        {
            if (previousStatus == ReservationStatus.Reserved && currentStatus == ReservationStatus.Completed)
                return "تم تاكيد الاقامة";

            if (previousStatus == ReservationStatus.Reserved
                && currentStatus is ReservationStatus.NoShow or ReservationStatus.Canceled)
            {
                return string.IsNullOrWhiteSpace(reservation.CancelationReason)
                    ? "لم يتم تحديد سبب"
                    : reservation.CancelationReason;
            }

            if (currentStatus == ReservationStatus.Reserved
                && previousStatus is ReservationStatus.NoShow or ReservationStatus.Canceled)
                return "تم إعادة تفعيل الحجز";

            return string.Empty;
        }
    }
}
