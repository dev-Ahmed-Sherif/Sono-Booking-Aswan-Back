using SonoTracker.Common.Core;
using SonoTracker.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Common.DTO.Tracker.TripMarina
{

    [ExcludeFromCodeCoverage]
    public class EditTripMarinaDto : IEntityDto<string>
    {
        public string Id { get; set; }

        public string TouristMarinaId { get; set; }

        public string TripInformationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }
    }
}
