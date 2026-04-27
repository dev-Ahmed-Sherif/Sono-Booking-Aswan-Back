using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.Inspection;
using SonoTracker.Common.DTO.Tracker.GeneralInspection.Parameters;
using SonoTracker.Common.DTO.Tracker.GeneralInspection;
using SonoTracker.Common.DTO.Tracker.Inspection.Parameters;

namespace SonoTracker.Application.Services.Tracker.Inspection
{
    public interface IInspectionService : IBaseService<Entities.Tracker.Inspection, AddInspectionDto, EditInspectionDto, InspectionDto, string, string>
    {
        Task<IFinalResult> GetAllAsync(string floatingUnitId, CancellationToken cancellationToken = default);

        Task<PagingResult> GetAllPagedAsync(BaseParam<InspectionFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> GetAllFilterAsync(InspectionFilter filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}
