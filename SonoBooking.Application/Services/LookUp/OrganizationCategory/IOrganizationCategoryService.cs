using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.OrganizationCategory;
using SonoTracker.Common.DTO.Lookup.OrganizationCategory.Parameters;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.LookUp.OrganizationCategory
{
    public interface IOrganizationCategoryService : IBaseService<Entities.Lookups.OrganizationCategory, AddOrganizationCategoryDto, EditOrganizationCategoryDto, OrganizationCategoryDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<OrganizationCategoryFilter> filter, CancellationToken cancellationToken = default);
        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
