using SonoBooking.Domain.Entities.Lookups;
using SonoTracker.Common.DTO.Lookup.AccidentType;

namespace SonoTracker.Application.Mapping
{
    public partial class MappingService
    {
        public void MapAccidentType()
        {
            CreateMap <BookingType, AccidentTypeDto>().ReverseMap();

            CreateMap<BookingType, EditAccidentTypeDto>().ReverseMap();
            
            CreateMap<AddAccidentTypeDto, BookingType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
