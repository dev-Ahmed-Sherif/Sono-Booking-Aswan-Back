using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.EmployeeJob;
using SonoBooking.Common.DTO.Lookup.EmployeeJob.Parameters;
using SonoBooking.Domain.Entities.Lookups;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.EmployeeJobs
{
    public interface IEmployeeJobService : IBaseService<EmployeeJob, AddEmployeeJobDto, EditEmployeeJobDto, EmployeeJobDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<EmployeeJobFilter> filter, CancellationToken cancellationToken = default);

        Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default);
    }
}
