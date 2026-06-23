using AutoMapper;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService : Profile
    {
        public MappingService()
        {
            MapAttachment();
            MapAllowedDayBeforeReservation();
            MapApartmentType();
            MapCity();
            MapEmployee();
            MapExtension();
            MapGovernorate();
            MapRelationship();
            MapRequestType();
            MapRoomType();
            MapTown();
            MapApproval();
            MapApartment();
            MapBed();
            MapCompanion();
            MapLeader();
            MapPayment();
            MapRequest();
            MapRequestParticipant();
            MapRequestUnit();
            MapRequestAttach();
            MapReservation();
            MapRoom();
            MapUnitImage();
        }
    }
}
