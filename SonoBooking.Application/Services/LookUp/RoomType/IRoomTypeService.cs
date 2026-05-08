using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.RoomType;
using SonoBooking.Common.DTO.Lookup.RoomType.Parameters;
using SonoBooking.Domain.Entities.Lookups;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.RoomType
{
    public interface IRoomTypeService : IBaseService<Entities.Lookups.RoomType, AddRoomTypeDto, EditRoomTypeDto, RoomTypeDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<RoomTypeFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
