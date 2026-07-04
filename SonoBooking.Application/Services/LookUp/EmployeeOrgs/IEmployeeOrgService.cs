using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.EmployeeOrg;
using SonoBooking.Common.DTO.Lookup.EmployeeOrg.Parameters;
using SonoBooking.Domain.Entities.Lookups;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.EmployeeOrgs
{
    public interface IEmployeeOrgService : IBaseService<EmployeeOrg, AddEmployeeOrgDto, EditEmployeeOrgDto, EmployeeOrgDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<EmployeeOrgFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
