using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;

namespace SonoTracker.Common.DTO.Tracker.TripInformation
{
    public class EditTripInformationDto : IEntityDto<string>
    {
        public string Id { get; set; }

        public DateTime SartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Code { get; set; }

        public string FloatingUnitId { get; set; }

        public int StaffNumber { get; set; }

        public int PassengerNumber { get; set; }

        public string RouteId { get; set; }
        public string PassengerAttachment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedById { get; set; }

    }
}
