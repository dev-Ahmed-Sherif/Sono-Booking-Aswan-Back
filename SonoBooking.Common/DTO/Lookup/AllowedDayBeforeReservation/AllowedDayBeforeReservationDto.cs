using SonoBooking.Common.DTO.Base;
using System;

namespace SonoBooking.Common.DTO.Lookup.AllowedDayBeforeReservation
{
    public class AllowedDayBeforeReservationDto : LookupDto<string>
    {
        public int NumofDays { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
