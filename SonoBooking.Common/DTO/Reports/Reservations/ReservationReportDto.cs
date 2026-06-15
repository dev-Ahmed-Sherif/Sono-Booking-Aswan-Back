namespace SonoBooking.Common.DTO.Reports.Reservations
{
    public class ReservationReportDto
    {
        public string RequestNumber { get; set; }
        public string RequestOwner { get; set; }
        public int Nights { get; set; }
        public int Apartments { get; set; }
        public int Rooms { get; set; }
        public int Beds { get; set; }
        public bool RequestStatus { get; set; }
        public bool ReservationStatus { get; set; }
        public float Revenue { get; set; }
        public string Notes { get; set; }
        public string User { get; set; }
        public string StartDateReport { get; set; }
        public string EndDateReport { get; set; }
    }
}

