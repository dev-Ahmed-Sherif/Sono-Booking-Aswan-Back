using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Extension;
using SonoBooking.Common.DTO.Housing.Extension.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Extensions
{
    public interface IExtensionService : IBaseService<Extension, AddExtensionDto, EditExtensionDto, ExtensionDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<ExtensionFilter> filter, CancellationToken cancellationToken = default);
    }
}
