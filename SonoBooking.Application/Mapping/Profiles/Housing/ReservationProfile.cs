using SonoBooking.Common.DTO.Housing.Reservation;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapReservation()
        {
            CreateMap<Reservation, ReservationDto>().ReverseMap();

            CreateMap<Reservation, EditReservationDto>().ReverseMap();

            CreateMap<AddReservationDto, Reservation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
