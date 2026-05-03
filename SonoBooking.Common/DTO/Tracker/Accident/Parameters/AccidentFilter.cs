using SonoBooking.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.Accident.Parameters
{
    [ExcludeFromCodeCoverage]
   public class AccidentFilter
    {
        public DateTime? AccidentDate { get; set; }
        public string CityId { get; set; }
        public string AccidentTypeId { get; set; }
        public Case? Case { get; set; }
        public string OrganizationId { get; set; }
        public string FloatingUnitId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
