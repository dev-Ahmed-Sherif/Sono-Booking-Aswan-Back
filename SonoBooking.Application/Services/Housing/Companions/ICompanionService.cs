using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Companion;
using SonoBooking.Common.DTO.Housing.Companion.Parameters;
using SonoBooking.Domain.Entities.Housing;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Companions
{
    public interface ICompanionService : IBaseService<Companion, AddCompanionDto, EditCompanionDto, CompanionDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<CompanionFilter> filter, CancellationToken cancellationToken = default);
    }
}
