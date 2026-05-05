using SonoBooking.Domain.Entities.Lookups;
using SonoTracker.Common.DTO.Lookup.UnitType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SonoTracker.Application.Mapping
{
  public partial class MappingService
    {
        public void MapUnitType()
        {
            //CreateMap<UnitType, UnitTypeDto>().ReverseMap();
            
            //CreateMap<UnitType, EditUnitTypeDto>().ReverseMap();

            //CreateMap<AddUnitTypeDto, UnitType>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ReverseMap();
        }
    }
}
