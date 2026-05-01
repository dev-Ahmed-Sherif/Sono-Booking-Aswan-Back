using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;

namespace SonoTracker.Common.DTO.Tracker.TripInformation
{
    public class AddTripInformationDto : IEntityDto<string>
    {
        public string? Id { get; set; }

        public DateTime SartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Code { get; set; }

        public string FloatingUnitId { get; set; }

        public int StaffNumber { get; set; }

        public int PassengerNumber { get; set; }

        public string RouteId { get; set; }
        //[MaxFileSize(5 * 1024 * 1024)]
        public IFormFile? PassengerAttachment { get; set; }
    }
}
