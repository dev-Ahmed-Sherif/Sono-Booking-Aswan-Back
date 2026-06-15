using System.ComponentModel.DataAnnotations;
using System;
using SonoBooking.Domain;

namespace SonoBooking.Common.DTO.Reports.Reservations
{
    public class FilterReservationReportDto : BaseReportSearch
    {
        [Required]
        public required DateOnly StartDate { get; set; }
        [Required]
        public required DateOnly EndDate { get; set; }
        public ReservationStatus? ReservationStatus { get; set; }
    }
}


