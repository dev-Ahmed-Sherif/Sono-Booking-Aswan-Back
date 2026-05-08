using System.ComponentModel.DataAnnotations;
using System;
using SonoBooking.Common.DTO.Reports;

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


