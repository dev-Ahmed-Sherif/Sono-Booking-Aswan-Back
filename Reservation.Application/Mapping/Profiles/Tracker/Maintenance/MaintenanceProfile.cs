using SonoTracker.Common.DTO.Tracker.Maintenance;
using SonoTracker.Common.DTO.Tracker.TouristMarina;
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
        public void MapMaintenance()
        {
            CreateMap<Maintenance, MaintenanceDto>().ReverseMap();
            CreateMap<Maintenance, EditMaintenanceDto>().ReverseMap();
            CreateMap<AddMaintenanceDto, Maintenance>()
                .ForMember(des => des.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
