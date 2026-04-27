using SonoTracker.Common.DTO.Base;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Lookup.Nationality
{
    [ExcludeFromCodeCoverage]
    public class EditNationalityDto : LookupDto<string>
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}
