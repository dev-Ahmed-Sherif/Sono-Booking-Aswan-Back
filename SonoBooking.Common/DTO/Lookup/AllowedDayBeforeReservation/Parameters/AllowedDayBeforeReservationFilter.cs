using SonoBooking.Common.DTO.Base;

namespace SonoBooking.Common.DTO.Lookup.AllowedDayBeforeReservation.Parameters
{
    public class AllowedDayBeforeReservationFilter : MainFilter
    {
        public int? NumofDays { get; set; }
    }
}
