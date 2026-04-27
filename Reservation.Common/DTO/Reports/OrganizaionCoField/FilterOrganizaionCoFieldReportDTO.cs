namespace SonoTracker.Common.DTO.Reports.OrganizaionCoField
{
    public class FilterOrganizaionCoFieldReportDTO 
    {
        public int[] CoFieldIds { get; set; } = [];
        public string CoFieldName { get; set; } = string.Empty;
        public int[] OrganizationIds { get; set; } = [];
        public string OrganizationName { get; set; }
        public string Description { get; set; }
    }
}
