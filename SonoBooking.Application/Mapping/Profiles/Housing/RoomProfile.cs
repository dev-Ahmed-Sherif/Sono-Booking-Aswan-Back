using SonoBooking.Common.DTO.Housing.Room;
using SonoBooking.Domain.Entities.Housing;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapRoom()
        {
            CreateMap<Room, RoomDto>().ReverseMap();

            CreateMap<Room, EditRoomDto>().ReverseMap();

            CreateMap<AddRoomDto, Room>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
