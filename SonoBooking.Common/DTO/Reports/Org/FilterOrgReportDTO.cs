using SonoBooking.Domain;
using SonoBooking.Common.DTO.Reports;
using System;

namespace SonoBooking.Common.DTO.Reports.Org
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


