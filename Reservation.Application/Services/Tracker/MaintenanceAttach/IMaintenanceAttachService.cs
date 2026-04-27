using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.MaintenanceAttach;
using SonoTracker.Common.DTO.Tracker.MaintenanceAttach.Parameters;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.MaintenanceAttach
{
    public interface IMaintenanceAttachService : IBaseService<Entities.Tracker.MaintenanceAttachment, AddMaintenanceAttachDto, EditMaintenanceAttachDto, MaintenanceAttachDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<MaintenanceAttachFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeWithAttachIdRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

        Task<IFinalResult> GetAllFilterAsync(MaintenanceAttachFilter filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
