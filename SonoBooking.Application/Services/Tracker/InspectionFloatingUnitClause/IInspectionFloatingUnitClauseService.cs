using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.InspectionFloatingUnitClause;
using SonoTracker.Common.DTO.Tracker.InspectionFloatingUnitClause.Parameters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.InspectionFloatingUnitClause
{
    public interface IInspectionFloatingUnitClauseService : IBaseService<Domain.Entities.Tracker.InspectionFloatingUnitClause, AddInspectionFloatingUnitClauseDto, EditInspectionFloatingUnitClauseDto, InspectionFloatingUnitClauseDto, string, string>
    {
        Task<IFinalResult> GetAllAsync(string? inspectionId, CancellationToken cancellationToken = default);

        Task<PagingResult> GetAllPagedAsync(BaseParam<InspectionFloatingUnitClauseFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}
