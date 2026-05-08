using LinqKit;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.DTO.Lookup.Relationship;
using SonoBooking.Common.DTO.Lookup.Relationship.Parameters;
using SonoBooking.Domain.Entities.Lookups;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SonoBooking.Domain;

namespace SonoBooking.Application.Services.LookUp.Relationship
{
    public class RelationshipService(IServiceBaseParameter<Entities.Lookups.Relationship> businessBaseParameter)
        : BaseService<Entities.Lookups.Relationship, AddRelationshipDto, EditRelationshipDto, RelationshipDto, string, string>(businessBaseParameter), IRelationshipService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Lookups.Relationship, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();

            IEnumerable<Entities.Lookups.Relationship> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                cancellationToken: cancellationToken);

            IEnumerable<Entities.Lookups.Relationship> filteredEntities = isSuperAdmin
                ? (entities ?? [])
                : (entities?.Where(x => !x.IsDeleted) ?? []);

            var mapped = Mapper.Map<IEnumerable<Entities.Lookups.Relationship>, IEnumerable<RelationshipDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<RelationshipFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var relationshipFilter = filter?.Filter ?? new RelationshipFilter();

            (int Count, IEnumerable<Entities.Lookups.Relationship> Result) =
                await UnitOfWork.Repository.FindPagedAsync(
                    predicate: PredicateBuilderFunction(relationshipFilter),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.Relationship>, IEnumerable<RelationshipDto>>(Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            (int Count, IEnumerable<Entities.Lookups.Relationship> Result) query =
                await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.Relationship>, IEnumerable<RelationshipDto>>(query.Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<Entities.Lookups.Relationship, bool>> PredicateBuilderFunction(RelationshipFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.Relationship>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.NameAr))
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            if (!string.IsNullOrWhiteSpace(filter.NameEn))
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));

            return predicate;
        }

        static Expression<Func<Entities.Lookups.Relationship, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.Relationship>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(x => x.NameEn.Contains(filter.SearchCriteria));
            }

            return predicate;
        }
    }
}
