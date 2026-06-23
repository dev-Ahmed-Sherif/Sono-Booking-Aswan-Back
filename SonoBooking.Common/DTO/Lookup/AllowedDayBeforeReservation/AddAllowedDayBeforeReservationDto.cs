using SonoBooking.Common.DTO.Base;

namespace SonoBooking.Common.DTO.Lookup.AllowedDayBeforeReservation
{
    public class AddAllowedDayBeforeReservationDto : LookupDto<string>
    {
        public int NumofDays { get; set; }
    }
}
