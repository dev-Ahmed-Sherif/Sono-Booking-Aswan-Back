using SonoTracker.Common.DTO.Identity.User;
using SonoTracker.Common.DTO.Tracker.Accident;
using SonoTracker.Common.DTO.Tracker.Maintenance;
using SonoTracker.Common.DTO.Tracker.TouristMarina;
using SonoTracker.Domain.Entities.Identity;
using SonoTracker.Domain.Entities.Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapUser()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
