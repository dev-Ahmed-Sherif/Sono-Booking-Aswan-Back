
using System;

namespace SonoBooking.Common.DTO.Reports.Requests
{
    public class RequestReportDto
    {
        public int TotalRequestCount { get; set; }
        public int TotalAcceptedRequestCount { get; set; }
        public int TotalRejectedRequestCount { get; set; }
        public int TotalAcceptedReservationCount { get; set; }
        public float TotalRevenue { get; set; }
        public string User { get; set; }
        public string StartDateReport { get; set; }
        public string EndDateReport { get; set; }

    }
}

