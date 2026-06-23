using SonoBooking.Domain.Entities.Base;
using System;

namespace SonoBooking.Domain.Entities.Lookups
{
    public class AllowedDayBeforeReservation : Lookup<string>
    {
        public int NumofDays { get; set; }

        public AllowedDayBeforeReservation()
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
    }
}
