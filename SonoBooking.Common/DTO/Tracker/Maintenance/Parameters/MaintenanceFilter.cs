using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.Maintenance.Parameters
{
    [ExcludeFromCodeCoverage]
   public class MaintenanceFilter
    {
        public int Number { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public string? MaintenanceTypeId { get; set; }
        public string? FloatingUnitId { get; set; }
        public bool IsDeleted { get; set; } = false;


    }
}
