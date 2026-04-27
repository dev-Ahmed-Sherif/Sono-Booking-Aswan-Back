using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Lookup.AccidentType;
using SonoTracker.Common.DTO.Lookup.InspectionType;
using SonoTracker.Domain.Entities.Lookups;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapInspectionType()
        {
            CreateMap<InspectionType, InspectionTypeDto>().ReverseMap();
            
            CreateMap<InspectionType, EditInspectionTypeDto>().ReverseMap();

            CreateMap<AddInspectionTypeDto, InspectionType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
