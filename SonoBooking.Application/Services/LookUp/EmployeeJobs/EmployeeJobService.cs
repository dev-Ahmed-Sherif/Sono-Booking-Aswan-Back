using LinqKit;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.EmployeeJob;
using SonoBooking.Common.DTO.Lookup.EmployeeJob.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.EmployeeJobs
{
    public class EmployeeJobService(IServiceBaseParameter<EmployeeJob> businessBaseParameter)
        : BaseService<EmployeeJob, AddEmployeeJobDto, EditEmployeeJobDto, EmployeeJobDto, string, string>(businessBaseParameter), IEmployeeJobService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<EmployeeJob, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();

            IEnumerable<EmployeeJob> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                cancellationToken: cancellationToken);

            IEnumerable<EmployeeJob> filteredEntities = isSuperAdmin
                ? (entities ?? [])
                : (entities?.Where(x => !x.IsDeleted) ?? []);

            var mapped = Mapper.Map<IEnumerable<EmployeeJob>, IEnumerable<EmployeeJobDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<EmployeeJobFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var employeeJobFilter = filter?.Filter ?? new EmployeeJobFilter();

            (int Count, IEnumerable<EmployeeJob> Result) =
                await UnitOfWork.Repository.FindPagedAsync(
                    predicate: PredicateBuilderFunction(employeeJobFilter),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<EmployeeJob>, IEnumerable<EmployeeJobDto>>(Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            (int Count, IEnumerable<EmployeeJob> Result) query =
                await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<EmployeeJob>, IEnumerable<EmployeeJobDto>>(query.Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<EmployeeJob, bool>> PredicateBuilderFunction(EmployeeJobFilter filter)
        {
            var predicate = PredicateBuilder.New<EmployeeJob>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.NameAr))
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            if (!string.IsNullOrWhiteSpace(filter.NameEn))
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));

            return predicate;
        }

        static Expression<Func<EmployeeJob, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<EmployeeJob>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(x => x.NameEn.Contains(filter.SearchCriteria));
            }

            return predicate;
        }
    }
}
