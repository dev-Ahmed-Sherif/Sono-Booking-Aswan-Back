using LinqKit;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.RoomType;
using SonoBooking.Common.DTO.Lookup.RoomType.Parameters;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Lookups;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.RoomType
{
    public class RoomTypeService(IServiceBaseParameter<Entities.Lookups.RoomType> businessBaseParameter)
        : BaseService<Entities.Lookups.RoomType, AddRoomTypeDto, EditRoomTypeDto, RoomTypeDto, string, string>(businessBaseParameter), IRoomTypeService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Lookups.RoomType, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();

            IEnumerable<Entities.Lookups.RoomType> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                cancellationToken: cancellationToken);

            IEnumerable<Entities.Lookups.RoomType> filteredEntities = isSuperAdmin
                ? (entities ?? [])
                : (entities?.Where(x => !x.IsDeleted) ?? []);

            var mapped = Mapper.Map<IEnumerable<Entities.Lookups.RoomType>, IEnumerable<RoomTypeDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<RoomTypeFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var roomTypeFilter = filter?.Filter ?? new RoomTypeFilter();

            (int Count, IEnumerable<Entities.Lookups.RoomType> Result) =
                await UnitOfWork.Repository.FindPagedAsync(
                    predicate: PredicateBuilderFunction(roomTypeFilter),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.RoomType>, IEnumerable<RoomTypeDto>>(Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;
            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            (int Count, IEnumerable<Entities.Lookups.RoomType> Result) query =
                await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.RoomType>, IEnumerable<RoomTypeDto>>(query.Result ?? []);
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<Entities.Lookups.RoomType, bool>> PredicateBuilderFunction(RoomTypeFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.RoomType>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.NameAr))
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            if (!string.IsNullOrWhiteSpace(filter.NameEn))
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));

            return predicate;
        }

        static Expression<Func<Entities.Lookups.RoomType, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.RoomType>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(x => x.NameEn.Contains(filter.SearchCriteria));
            }

            return predicate;
        }
    }
}
