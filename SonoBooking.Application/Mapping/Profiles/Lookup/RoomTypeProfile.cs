using SonoBooking.Common.DTO.Lookup.RoomType;
using SonoBooking.Domain.Entities.Lookups;

namespace SonoBooking.Application.Mapping
{
    public partial class MappingService
    {
        public void MapRoomType()
        {
            CreateMap<RoomType, RoomTypeDto>().ReverseMap();

            CreateMap<RoomType, EditRoomTypeDto>().ReverseMap();

            CreateMap<AddRoomTypeDto, RoomType>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
