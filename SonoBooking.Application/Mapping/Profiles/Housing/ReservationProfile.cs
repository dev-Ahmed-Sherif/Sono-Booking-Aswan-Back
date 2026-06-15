using SonoBooking.Common.DTO.Housing.Reservation;
using SonoBooking.Common.DTO.Reports.Reservations;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Linq;

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

            CreateMap<Reservation, ReservationReportDto>()
                .ForMember(dest => dest.RequestNumber, opt => opt.MapFrom(src => src.Request != null ? src.Request.RequestNumber : null))
                .ForMember(dest => dest.RequestOwner, opt => opt.MapFrom(src => src.Request != null && src.Request.User != null ? src.Request.User.FullName : null))
                .ForMember(dest => dest.Nights, opt => opt.MapFrom(src => CalculateReservationNights(src)))
                .ForMember(dest => dest.Apartments, opt => opt.MapFrom(src => CountRequestUnits(src, unit => !string.IsNullOrWhiteSpace(unit.ApartmentId))))
                .ForMember(dest => dest.Rooms, opt => opt.MapFrom(src => CountRequestUnits(src, unit => !string.IsNullOrWhiteSpace(unit.RoomId))))
                .ForMember(dest => dest.Beds, opt => opt.MapFrom(src => CountRequestUnits(src, unit => !string.IsNullOrWhiteSpace(unit.BedId))))
                .ForMember(dest => dest.RequestStatus, opt => opt.MapFrom(src => src.Request != null && src.Request.Status == Status.Approved))
                .ForMember(dest => dest.ReservationStatus, opt => opt.MapFrom(src =>
                    src.Status != ReservationStatus.Canceled && src.Status != ReservationStatus.NoShow))
                .ForMember(dest => dest.Revenue, opt => opt.MapFrom(src => (float)src.TotalAmount))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.CancelationReason ?? string.Empty))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.StartDateReport, opt => opt.Ignore())
                .ForMember(dest => dest.EndDateReport, opt => opt.Ignore());
        }

        private static int CalculateReservationNights(Reservation reservation)
        {
            int nights = reservation.EndDate.DayNumber - reservation.StartDate.DayNumber;
            return nights <= 0 ? 1 : nights;
        }

        private static int CountRequestUnits(Reservation reservation, System.Func<RequestUnit, bool> predicate)
        {
            if (reservation.Request?.RequestUnits == null)
                return 0;

            return reservation.Request.RequestUnits.Count(unit => !unit.IsDeleted && predicate(unit));
        }
    }
}
