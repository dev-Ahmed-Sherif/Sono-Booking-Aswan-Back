using SonoBooking.Common.DTO.Housing.Reservation;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapReservation()
        {
            CreateMap<Reservation, ReservationDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Request != null ? src.Request.UserId : null));

            CreateMap<ReservationDto, Reservation>()
                .ForMember(dest => dest.Request, opt => opt.Ignore());

            CreateMap<Reservation, EditReservationDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Request != null ? src.Request.UserId : null));

            CreateMap<EditReservationDto, Reservation>()
                .ForMember(dest => dest.Request, opt => opt.Ignore());

            CreateMap<AddReservationDto, Reservation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Payment, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
