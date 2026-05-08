using LinqKit;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.Governorate;
using SonoBooking.Common.DTO.Lookup.Governorate.Parameters;
using SonoBooking.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using GovernorateEntity = SonoBooking.Domain.Entities.Lookups.Governorate;

namespace SonoBooking.Application.Services.LookUp.Governorate
{
    public class GovernorateService(IServiceBaseParameter<GovernorateEntity> businessBaseParameter)
        : BaseService<GovernorateEntity, AddGovernorateDto, EditGovernorateDto, GovernorateDto, string, string>(businessBaseParameter), IGovernorateService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<GovernorateEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();

            IEnumerable<GovernorateEntity> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                cancellationToken: cancellationToken);

            IEnumerable<GovernorateEntity> filteredEntities = isSuperAdmin
                ? (entities ?? [])
                : (entities?.Where(x => !x.IsDeleted) ?? []);

            var mapped = Mapper.Map<IEnumerable<GovernorateEntity>, IEnumerable<GovernorateDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<GovernorateFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var governorateFilter = filter?.Filter ?? new GovernorateFilter();

            if (!isSuperAdmin)
                governorateFilter.IsDeleted = false;

            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;

            (int Count, IEnumerable<GovernorateEntity> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: PredicateBuilderFunction(governorateFilter),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            var filteredResult = isSuperAdmin
                ? (Result ?? [])
                : (Result?.Where(x => x.IsDeleted != true) ?? []);

            var data = Mapper.Map<IEnumerable<GovernorateEntity>, IEnumerable<GovernorateDto>>(filteredResult);

            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(
                predicate: DropDownPredicateBuilderFunction(filter.Filter),
                pageNumber: offset,
                pageSize: limit,
                cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<GovernorateEntity>, IEnumerable<GovernorateDto>>(query.Result.Where(x => x.IsDeleted != true));

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<GovernorateEntity, bool>> PredicateBuilderFunction(GovernorateFilter filter)
        {
            var predicate = PredicateBuilder.New<GovernorateEntity>(x => x.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.NameAr))
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));

            if (!string.IsNullOrWhiteSpace(filter.NameEn))
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));

            return predicate;
        }

        static Expression<Func<GovernorateEntity, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<GovernorateEntity>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(x => x.NameEn.Contains(filter.SearchCriteria));
            }

            return predicate;
        }
    }
}
