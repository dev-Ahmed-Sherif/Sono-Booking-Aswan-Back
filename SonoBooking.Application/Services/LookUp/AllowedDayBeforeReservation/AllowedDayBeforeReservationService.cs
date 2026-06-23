using LinqKit;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.AllowedDayBeforeReservation;
using SonoBooking.Common.DTO.Lookup.AllowedDayBeforeReservation.Parameters;
using SonoBooking.Domain;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.AllowedDayBeforeReservation
{
    public class AllowedDayBeforeReservationService(IServiceBaseParameter<Entities.Lookups.AllowedDayBeforeReservation> businessBaseParameter)
        : BaseService<Entities.Lookups.AllowedDayBeforeReservation, AddAllowedDayBeforeReservationDto, EditAllowedDayBeforeReservationDto, AllowedDayBeforeReservationDto, string, string>(businessBaseParameter), IAllowedDayBeforeReservationService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Lookups.AllowedDayBeforeReservation, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();

            IEnumerable<Entities.Lookups.AllowedDayBeforeReservation> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                cancellationToken: cancellationToken);

            IEnumerable<Entities.Lookups.AllowedDayBeforeReservation> filteredEntities = isSuperAdmin
                ? (entities ?? [])
                : (entities?.Where(x => !x.IsDeleted) ?? []);

            var mapped = Mapper.Map<IEnumerable<Entities.Lookups.AllowedDayBeforeReservation>, IEnumerable<AllowedDayBeforeReservationDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<AllowedDayBeforeReservationFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var allowedDayFilter = filter?.Filter ?? new AllowedDayBeforeReservationFilter();

            (int Count, IEnumerable<Entities.Lookups.AllowedDayBeforeReservation> Result) =
                await UnitOfWork.Repository.FindPagedAsync(
                    predicate: PredicateBuilderFunction(allowedDayFilter),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.AllowedDayBeforeReservation>, IEnumerable<AllowedDayBeforeReservationDto>>(Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            (int Count, IEnumerable<Entities.Lookups.AllowedDayBeforeReservation> Result) query =
                await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.AllowedDayBeforeReservation>, IEnumerable<AllowedDayBeforeReservationDto>>(query.Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<Entities.Lookups.AllowedDayBeforeReservation, bool>> PredicateBuilderFunction(AllowedDayBeforeReservationFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.AllowedDayBeforeReservation>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.NameAr))
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            if (!string.IsNullOrWhiteSpace(filter.NameEn))
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));
            if (filter.NumofDays.HasValue)
                predicate = predicate.And(x => x.NumofDays == filter.NumofDays.Value);

            return predicate;
        }

        static Expression<Func<Entities.Lookups.AllowedDayBeforeReservation, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.AllowedDayBeforeReservation>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(x => x.NameEn.Contains(filter.SearchCriteria));
            }

            return predicate;
        }
    }
}
