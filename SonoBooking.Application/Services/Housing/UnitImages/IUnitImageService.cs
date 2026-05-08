using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.UnitImage;
using SonoBooking.Common.DTO.Housing.UnitImage.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.UnitImages
{
    public interface IUnitImageService : IBaseService<UnitImage, AddUnitImageDto, EditUnitImageDto, UnitImageDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<UnitImageFilter> filter, CancellationToken cancellationToken = default);
    }
}
