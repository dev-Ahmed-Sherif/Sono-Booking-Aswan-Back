using LinqKit;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.EmployeeOrg;
using SonoBooking.Common.DTO.Lookup.EmployeeOrg.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.EmployeeOrgs
{
    public class EmployeeOrgService(IServiceBaseParameter<EmployeeOrg> businessBaseParameter)
        : BaseService<EmployeeOrg, AddEmployeeOrgDto, EditEmployeeOrgDto, EmployeeOrgDto, string, string>(businessBaseParameter), IEmployeeOrgService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<EmployeeOrg, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();

            IEnumerable<EmployeeOrg> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                cancellationToken: cancellationToken);

            IEnumerable<EmployeeOrg> filteredEntities = isSuperAdmin
                ? (entities ?? [])
                : (entities?.Where(x => !x.IsDeleted) ?? []);

            var mapped = Mapper.Map<IEnumerable<EmployeeOrg>, IEnumerable<EmployeeOrgDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<EmployeeOrgFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var employeeOrgFilter = filter?.Filter ?? new EmployeeOrgFilter();

            (int Count, IEnumerable<EmployeeOrg> Result) =
                await UnitOfWork.Repository.FindPagedAsync(
                    predicate: PredicateBuilderFunction(employeeOrgFilter),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<EmployeeOrg>, IEnumerable<EmployeeOrgDto>>(Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            (int Count, IEnumerable<EmployeeOrg> Result) query =
                await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<EmployeeOrg>, IEnumerable<EmployeeOrgDto>>(query.Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<EmployeeOrg, bool>> PredicateBuilderFunction(EmployeeOrgFilter filter)
        {
            var predicate = PredicateBuilder.New<EmployeeOrg>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.NameAr))
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            if (!string.IsNullOrWhiteSpace(filter.NameEn))
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));

            return predicate;
        }

        static Expression<Func<EmployeeOrg, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<EmployeeOrg>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(x => x.NameEn.Contains(filter.SearchCriteria));
            }

            return predicate;
        }
    }
}
