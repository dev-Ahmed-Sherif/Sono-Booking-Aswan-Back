using SonoBooking.Domain;
using System;
using System.Globalization;

namespace SonoBooking.Application.Services.Housing.Notifications
{
    public static class HousingNotificationMessages
    {
        private static readonly CultureInfo DisplayCulture = CultureInfo.GetCultureInfo("ar-EG");

        public static string FormatTimestamp(DateTime utc) =>
            utc.ToLocalTime().ToString("d MMMM yyyy، h:mm tt", DisplayCulture);

        public static string FormatDate(DateOnly date) =>
            date.ToString("dd/MM/yyyy", DisplayCulture);

        public static string NewRequest(string requestNumber, string ownerName, DateTime requestDate) =>
            $"طلب جديد رقم {requestNumber} من {ownerName} — {FormatTimestamp(requestDate)}";

        public static string RequestApproved(string requestNumber, string leaderName) =>
            $"تمت الموافقة على طلبك رقم {requestNumber} من قبل {leaderName}";

        public static string RequestRejected(string requestNumber) =>
            $"تم رفض طلبك رقم {requestNumber}";

        public static string NewReservationForReception(
            string requestNumber,
            DateOnly checkIn,
            DateOnly checkOut) =>
            $"حجز جديد لطلب رقم {requestNumber} — تاريخ الوصول {FormatDate(checkIn)}، تاريخ المغادرة {FormatDate(checkOut)}";

        public static string ReservationStatusUpdated(string requestNumber, ReservationStatus status) =>
            $"تم تحديث حالة الحجز لطلب رقم {requestNumber} إلى {FormatReservationStatus(status)}";

        public static string FormatReservationStatus(ReservationStatus status) =>
            status switch
            {
                ReservationStatus.Reserved => "محجوز",
                ReservationStatus.Completed => "تم اكتمال الإقامة",
                ReservationStatus.Canceled => "ملغى",
                ReservationStatus.NoShow => "لم يظهر",
                ReservationStatus.Checkout => "مقبول",
                _ => status.ToString()
            };
    }
}
