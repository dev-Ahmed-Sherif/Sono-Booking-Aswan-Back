using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.RequestUnit;
using SonoBooking.Common.DTO.Housing.RequestUnit.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.RequestUnits
{
    public interface IRequestUnitService : IBaseService<RequestUnit, AddRequestUnitDto, EditRequestUnitDto, RequestUnitDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<RequestUnitFilter> filter, CancellationToken cancellationToken = default);
    }
}
