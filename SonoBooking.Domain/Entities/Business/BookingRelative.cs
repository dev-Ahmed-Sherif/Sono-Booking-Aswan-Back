using SonoTracker.Domain.Entities.Base;
using SonoTracker.Domain.Entities.Identity;
using SonoTracker.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoBooking.Domain.Entities.Business
{
    public class BookingCompanion : BaseEntity<string>
    {
        public BookingCompanion() 
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(Booking))]
        public required string BookingId { get; set; }
        public virtual Booking? Booking { get; set; }
    }
}
