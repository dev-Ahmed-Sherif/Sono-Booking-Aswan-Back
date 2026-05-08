using System;

namespace SonoBooking.Common.DTO.Reports.OrganizaionCoField
{
    public class OrganizaionCoFieldReportDto
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public int CoFieldId { get; set; }
        public string CoFieldName { get; set; }
        public string Description { get; set; } = string.Empty;
        public int OrganizationCityId { get; set; }
        public string OrganizationCityName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string User { get; set; }
        public int CheckOrgAndCitySelect { get; set; }
    }
}

