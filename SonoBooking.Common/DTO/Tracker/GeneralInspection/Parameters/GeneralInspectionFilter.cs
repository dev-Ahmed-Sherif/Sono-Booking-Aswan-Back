using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.GeneralInspection.Parameters
{
    public class GeneralInspectionFilter
    {

        public DateTime? InspectionDate { get; set; }

        public string? OrganizationId { get; set; }

        public string? FloatingUnitId { get; set; }
        public bool IsInspected { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
