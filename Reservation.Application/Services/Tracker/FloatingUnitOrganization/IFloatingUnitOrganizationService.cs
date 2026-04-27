using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Tracker.FloatingUnitOrganization;
using SonoTracker.Common.DTO.Tracker.FloatingUnitOrganization.Parameters;

namespace SonoTracker.Application.Services.Tracker.FloatingUnitOrganization
{
    public interface IFloatingUnitOrganizationService : IBaseService<Domain.Entities.Tracker.FloatingUnitOrganization, AddFloatingUnitOrganizationDto, EditFloatingUnitOrganizationDto, FloatingUnitOrganizationDto, string, string>
    {
        Task<IFinalResult> GetAllAsync(string? floatingUnitId, CancellationToken cancellationToken = default);

        Task<PagingResult> GetAllPagedAsync(BaseParam<FloatingUnitOrganizationFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

    }
}
