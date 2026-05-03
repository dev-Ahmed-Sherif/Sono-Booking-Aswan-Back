using SonoBooking.Domain;
using SonoTracker.Common.DTO.ReportsDTOs;
using System;

namespace SonoTracker.Common.DTO.Reports.Org
{
    public class FilterOrgReportDTO : BaseReportSearch
    {
      
        public string[]? OrganizationIds { get; set; } = [];

        public string NameAr { get; set; } = string.Empty;

        public string NameEn { get; set; } = string.Empty;

        public OrganizationType? OrganizationTypeId { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
