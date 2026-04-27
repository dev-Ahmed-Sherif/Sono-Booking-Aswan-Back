using AutoMapper;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.Maintenance;
using SonoTracker.Common.DTO.Tracker.Maintenance.Parameters;
using SonoTracker.Infrastructure.UnitOfWork;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.Maintenance
{
   public  interface IMaintenanceService : IBaseService<Entities.Tracker.Maintenance, AddMaintenanceDto, EditMaintenanceDto, MaintenanceDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<MaintenanceFilter> Filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetAllFilterAsync(MaintenanceFilter filter, CancellationToken cancellationToken = default);
        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}
