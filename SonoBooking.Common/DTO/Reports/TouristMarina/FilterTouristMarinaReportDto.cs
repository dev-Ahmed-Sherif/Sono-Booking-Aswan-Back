using System.ComponentModel.DataAnnotations;
using System;
using SonoTracker.Common.DTO.ReportsDTOs;

namespace SonoBooking.Common.DTO.Reports.TouristMarina
{
    public class FilterTouristMarinaReportDto : BaseReportSearch
    {
        public string? TouristMarinaId { get; set; }

        public string? OrganizationId { get; set; }
        public string? TownId { get; set; }
         public bool IsDeleted { get; set; } = false;

    }
}
