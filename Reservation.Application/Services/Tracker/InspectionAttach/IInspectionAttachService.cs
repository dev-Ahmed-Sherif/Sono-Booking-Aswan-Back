using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.InspectionAttach;
using SonoTracker.Common.DTO.Tracker.InspectionAttach.Parameters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.InspectionAttach
{
    public interface IInspectionAttachService : IBaseService<Entities.Tracker.InspectionAttachment, AddInspectionAttachDto, EditInspectionAttachDto, InspectionAttachDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<InspectionAttachFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeWithAttachIdRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

        Task<IFinalResult> GetAllFilterAsync(InspectionAttachFilter filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
