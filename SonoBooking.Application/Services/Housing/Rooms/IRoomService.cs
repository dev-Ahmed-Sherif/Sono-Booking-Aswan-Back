using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Room;
using SonoBooking.Common.DTO.Housing.Room.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Rooms
{
    public interface IRoomService : IBaseService<Room, AddRoomDto, EditRoomDto, RoomDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<RoomFilter> filter, CancellationToken cancellationToken = default);
    }
}
