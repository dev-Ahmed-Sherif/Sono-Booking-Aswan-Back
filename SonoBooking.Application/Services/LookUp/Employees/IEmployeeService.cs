using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.Employee;
using SonoBooking.Common.DTO.Lookup.Employee.Parameters;
using SonoBooking.Domain.Entities.Lookups;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.Employees
{
    public interface IEmployeeService : IBaseService<Employee, AddEmployeeDto, EditEmployeeDto, EmployeeDto, string, string>
    {
        Task<PagingResult> GetAllPagedAsync(BaseParam<EmployeeFilter> filter, CancellationToken cancellationToken = default);
    }
}
