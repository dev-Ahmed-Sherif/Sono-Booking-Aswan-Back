using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Reservation
{
    [ExcludeFromCodeCoverage]
    public class EditReservationDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? ActualCheckOutDate { get; set; }
        public ReservationStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string RequestId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
