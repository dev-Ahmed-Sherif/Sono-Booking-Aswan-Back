using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.FloatingUnitStaff;
using SonoTracker.Common.DTO.Tracker.FloatingUnitStaff.Parameters;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.FloatingUnitStaff
{
    public interface IFloatingUnitStaffService : IBaseService<Domain.Entities.Tracker.FloatingUnitStaff, AddFloatingUnitStaffDto, EditFloatingUnitStaffDto, FloatingUnitStaffDto, string, string>
    {
        Task<IFinalResult> GetAllAsync(string? floatingUnitId, CancellationToken cancellationToken = default);

        Task<PagingResult> GetAllPagedAsync(BaseParam<FloatingUnitStaffFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);

    }
}
