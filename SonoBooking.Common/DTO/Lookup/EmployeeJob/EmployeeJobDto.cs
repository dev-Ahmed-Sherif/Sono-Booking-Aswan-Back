using SonoBooking.Common.DTO.Base;
using System;

namespace SonoBooking.Common.DTO.Lookup.EmployeeJob
{
    public class EmployeeJobDto : LookupDto<string>
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedById { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedById { get; set; }
        public string ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
