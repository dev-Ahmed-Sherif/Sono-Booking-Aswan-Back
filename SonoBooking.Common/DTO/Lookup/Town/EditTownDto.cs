using SonoBooking.Common.DTO.Base;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Lookup.Town
{
    [ExcludeFromCodeCoverage]
    public class EditTownDto : LookupDto<string>
    {
        public string CityId { get; set; }
        public string GovernorateId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }

    }
}


