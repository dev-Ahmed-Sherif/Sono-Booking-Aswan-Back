using LinqKit;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.Employee;
using SonoBooking.Common.DTO.Lookup.Employee.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.Employees
{
    public class EmployeeService(IServiceBaseParameter<Employee> businessBaseParameter)
        : BaseService<Employee, AddEmployeeDto, EditEmployeeDto, EmployeeDto, string, string>(businessBaseParameter), IEmployeeService
    {
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<EmployeeFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            EmployeeFilter employeeFilter = filter?.Filter ?? new EmployeeFilter();

            (int Count, IEnumerable<Employee> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: PredicateBuilderFunction(employeeFilter),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<EmployeeDto> data = Mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<Employee, bool>> PredicateBuilderFunction(EmployeeFilter filter)
        {
            var predicate = PredicateBuilder.New<Employee>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.Name))
                predicate = predicate.And(x => x.Name.Contains(filter.Name));
            if (!string.IsNullOrWhiteSpace(filter.NationalId))
                predicate = predicate.And(x => x.NationalId.Contains(filter.NationalId));
            if (!string.IsNullOrWhiteSpace(filter.EmployeeOrgId))
                predicate = predicate.And(x => x.EmployeeOrgId == filter.EmployeeOrgId);
            if (!string.IsNullOrWhiteSpace(filter.EmployeeJobId))
                predicate = predicate.And(x => x.EmployeeJobId == filter.EmployeeJobId);

            return predicate;
        }
    }
}
