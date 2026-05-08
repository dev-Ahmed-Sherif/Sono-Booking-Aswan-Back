using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Bed;
using SonoBooking.Common.DTO.Housing.Bed.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Beds
{
    public interface IBedService : IBaseService<Bed, AddBedDto, EditBedDto, BedDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<BedFilter> filter, CancellationToken cancellationToken = default);
    }
}
