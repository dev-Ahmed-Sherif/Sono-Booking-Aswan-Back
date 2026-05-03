namespace SonoBooking.Common.DTO.Reports.Org
{
    public class OrgReportDTO
    {
      
        
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; }
        public string NameAr { get; set; }

        public string NameEn { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Fax { get; set; }
        //public Guid NationalityId { get; set; }
        public string NationalityName { get; set; } = "";
        public int? TouristMarinaNumber { get; set; }

        public string User { get; set; } 
        //public EnumResult OrganizationTypeId { get; set; }

    }
   
}
