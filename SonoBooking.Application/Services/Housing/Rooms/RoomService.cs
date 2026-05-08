using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Room;
using SonoBooking.Common.DTO.Housing.Room.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Rooms
{
    public class RoomService(IServiceBaseParameter<Room> businessBaseParameter) : BaseService<Room, AddRoomDto, EditRoomDto, RoomDto, string, string>(businessBaseParameter), IRoomService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<RoomFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            RoomFilter roomFilter = filter?.Filter ?? new RoomFilter();

            (int Count, IEnumerable<Room> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == roomFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<RoomDto> data = Mapper.Map<IEnumerable<Room>, IEnumerable<RoomDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
    }
}
