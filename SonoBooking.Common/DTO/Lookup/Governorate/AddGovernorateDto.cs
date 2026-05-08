using SonoBooking.Common.DTO.Base;

namespace SonoBooking.Common.DTO.Lookup.Governorate
{
    public class AddGovernorateDto : LookupDto<string>
    {
        public string WebsiteUrl { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
    }
}


