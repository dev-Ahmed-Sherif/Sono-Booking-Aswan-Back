using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Domain.Entities.Lookups;
using SonoTracker.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoBooking.Domain.Entities.Business
{
    public class Booking : BaseAudit<string>
    {
        public Booking() 
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.CreateVersion7().ToString();
            }
        }
        
        [Required]
        public required DateOnly BookingDate { get; set; }
        public DateOnly? ExtendDate { get; set; }
        
        [Required]
        public required int NightsCount { get; set; }
        public int? ExtendNights { get; set; }
        public Status Status { get; set; } = Status.Pending;
        public bool NeedExtend { get; set; } = false;

        [Required, MaxLength(50)]
        [ForeignKey(nameof(BookingType))]
        public string BookingTypeId { get; set; }
        public virtual BookingType? BookingType { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public virtual User? User { get; set; }

        [Required, MaxLength(50)]
        [ForeignKey(nameof(Governorate))]
        public string GovernorateId { get; set; }
        public virtual Governorate? Governorate { get; set; }
    }
}
