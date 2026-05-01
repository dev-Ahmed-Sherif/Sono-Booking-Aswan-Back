using SonoTracker.Common.DTO.Lookup.AccidentType;
using SonoTracker.Domain.Entities.Lookups;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapAccidentType()
        {
            CreateMap <AccidentType, AccidentTypeDto>().ReverseMap();

            CreateMap<AccidentType, EditAccidentTypeDto>().ReverseMap();
            
            CreateMap<AddAccidentTypeDto, AccidentType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
