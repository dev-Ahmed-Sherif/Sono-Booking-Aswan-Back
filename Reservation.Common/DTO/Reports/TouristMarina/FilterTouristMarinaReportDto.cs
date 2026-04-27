using SonoTracker.Domain.Entities.Tracker;
using System.ComponentModel.DataAnnotations;
using System;
using SonoTracker.Domain.Entities.Lookups;
using SonoTracker.Common.DTO.ReportsDTOs;

namespace SonoTracker.Common.DTO.Reports.TouristMarina
{
    public class FilterTouristMarinaReportDto : BaseReportSearch
    {
        public string? TouristMarinaId { get; set; }

        public string? OrganizationId { get; set; }
        public string? TownId { get; set; }
         public bool IsDeleted { get; set; } = false;

    }
}
