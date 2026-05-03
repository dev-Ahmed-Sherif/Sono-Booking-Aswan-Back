using System;
using System.Diagnostics.CodeAnalysis;
using SonoTracker.Common.DTO.Base;
using SonoBooking.Domain;

namespace SonoTracker.Common.DTO.Lookup.UnitType
{
     [ExcludeFromCodeCoverage]
     public class UnitTypeDto : LookupDto<string>
     {
        public UnitCategory UnitCategory { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
     }
}
