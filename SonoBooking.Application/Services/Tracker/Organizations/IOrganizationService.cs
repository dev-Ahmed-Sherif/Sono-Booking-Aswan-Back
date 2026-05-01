using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Reports.Org;
using SonoTracker.Common.DTO.Tracker.Organization;
using SonoTracker.Common.DTO.Tracker.Organization.Parameters;
using SonoTracker.Domain.Entities.Lookups;
using SonoTracker.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.Organizations
{
    public interface IOrganizationService : IBaseService<Entities.Tracker.Organization, AddOrganizationDto, EditOrganizationDto, OrganizationDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<OrganizationFilter> filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetAllAsync(OrganizationType? organizationTypeId, string organizationCategoryId, CancellationToken cancellationToken = default);
        Task<IFinalResult> GetFilterAsync(OrganizationFilter filter, CancellationToken cancellationToken = default);
        Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);

        Task<IFinalResult> GetAllReportAsync(FilterOrgReportDTO filter, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateReportAsync(FilterOrgReportDTO filter, CancellationToken cancellationToken = default);
    }
}
