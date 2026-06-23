using SonoBooking.Common.DTO.Lookup.AllowedDayBeforeReservation;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapAllowedDayBeforeReservation()
        {
            CreateMap<AllowedDayBeforeReservation, AllowedDayBeforeReservationDto>().ReverseMap();

            CreateMap<AllowedDayBeforeReservation, EditAllowedDayBeforeReservationDto>().ReverseMap();

            CreateMap<AddAllowedDayBeforeReservationDto, AllowedDayBeforeReservation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
