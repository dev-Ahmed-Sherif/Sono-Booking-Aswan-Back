using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.OrganizationStaff.Parameters;
using SonoTracker.Common.DTO.Tracker.OrganizationStaff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.OrganizationStaffStaff
{
    public interface IOrganizationStaffService : IBaseService<Domain.Entities.Tracker.OrganizationStaff, AddOrganizationStaffDto, EditOrganizationStaffDto, OrganizationStaffDto, string, string>
    {
        Task<IFinalResult> GetAllAsync(string? organizationId, CancellationToken cancellationToken = default);
        Task<PagingResult> GetAllPagedAsync(BaseParam<OrganizationStaffFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
