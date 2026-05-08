using AutoMapper;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService : Profile
    {
        public MappingService()
        {
            MapAttachment();
            MapApartmentType();
            MapCity();
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
            MapReservation();
            MapRoom();
            MapUnitImage();
        }
    }
}
