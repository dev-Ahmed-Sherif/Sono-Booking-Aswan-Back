using SonoBooking.Common.Core;
using SonoBooking.Domain;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Reservation
{
    [ExcludeFromCodeCoverage]
    public class AddReservationDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateTime? CheckInDate { get; set; }
        public DateTime? ActualCheckOutDate { get; set; }
        public ReservationStatus Status { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string UserId { get; set; }
        public string RequestId { get; set; }
        public string? ApartmentId { get; set; }
        public string? RoomId { get; set; }
        public string? BedId { get; set; }
    }
}
