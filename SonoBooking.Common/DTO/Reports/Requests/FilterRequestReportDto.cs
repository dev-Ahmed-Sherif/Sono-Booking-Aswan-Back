using System.ComponentModel.DataAnnotations;
using System;

namespace SonoBooking.Common.DTO.Reports.Requests
{
    public class FilterRequestReportDto : BaseReportSearch
    {
        [Required]
        public required DateOnly StartDate { get; set; }
        [Required]
        public required DateOnly EndDate { get; set; }
        public string? RequestId { get; set; }
    }
}


