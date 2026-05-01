using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LinqKit;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Domain;
using Microsoft.EntityFrameworkCore;
using SonoTracker.Common.DTO.Tracker.TripGeo;
using SonoTracker.Common.DTO.Tracker.TripGeo.Parameters;

namespace SonoTracker.Application.Services.Tracker.TripGeo
{
    public class TripGeoService(IServiceBaseParameter<Entities.Tracker.TripGeo> businessBaseParameter) : BaseService<Entities.Tracker.TripGeo, AddTripGeoDto, EditTripGeoDto, TripGeoDto, string, string>(businessBaseParameter), ITripGeoService
    {
        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src.Include(t => t.GeoPoint)
                .Include(t => t.TripInformation).ThenInclude(t => t.FloatingUnit), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.TripGeo, EditTripGeoDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src.Include(t => t.GeoPoint)
                .Include(t => t.TripInformation).ThenInclude(t => t.FloatingUnit), cancellationToken: cancellationToken);

            var mapped = Mapper.Map<Entities.Tracker.TripGeo, EditTripGeoDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public async Task<IFinalResult> GetLastByFloatingUnitIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository
                    .LastOrDefaultAsync(x => x.TripInformation.FloatingUnitId == idStr,
                            //orderBy: q => q.OrderByDescending(d => d.),
                            include: src => src
                                       .Include(t => t.GeoPoint)
                                       .Include(t => t.TripInformation)
                                       .ThenInclude(t => t.FloatingUnit),
                            cancellationToken: cancellationToken);

            var mapped = Mapper.Map<Entities.Tracker.TripGeo, EditTripGeoDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.TripGeo, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.GetAllAsync(include: src => src.Include(t => t.GeoPoint)
             .Include(t => t.TripInformation)
             .ThenInclude(t => t.FloatingUnit)
,               cancellationToken: cancellationToken);
            var filteredEntities = IsSuperAdmin()
                ? entity
                : entity.Where(e => e.IsDeleted != true);
            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.TripGeo>, IEnumerable<TripGeoDto>>(filteredEntities);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<TripGeoFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var tripGeoFilter = filter?.Filter ?? new TripGeoFilter();
            if (!isSuperAdmin)
                tripGeoFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(tripGeoFilter),
                pageNumber: offset, pageSize: limit,
                filter.OrderByValue,
                include: src => src.Include(t => t.GeoPoint)
                .Include(t => t.TripInformation).ThenInclude(t => t.FloatingUnit),
                cancellationToken: cancellationToken);

            var items = isSuperAdmin ? query.Item2 : query.Item2.Where(x => x.IsDeleted != true);
            var data = Mapper.Map<IEnumerable<Entities.Tracker.TripGeo>, IEnumerable<TripGeoDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var items = isSuperAdmin ? query.Item2 : query.Item2.Where(x => x.IsDeleted != true);
            var data = Mapper.Map<IEnumerable<Entities.Tracker.TripGeo>, IEnumerable<TripGeoDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);

        }
        public async Task<IFinalResult> GetAllFilterAsync(TripGeoFilter filter, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FindAsync(predicate: PredicateBuilderFunction(filter), cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.TripGeo>, IEnumerable<TripGeoDto>>(entity.Where(x => x.IsDeleted != true));

            return ResponseResult.PostResult(data, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
        static Expression<Func<Entities.Tracker.TripGeo, bool>> PredicateBuilderFunction(TripGeoFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.TripGeo>(x => x.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.TripInformationId))
            {
                predicate = predicate.And(x => x.TripInformationId == filter.TripInformationId);
            }
            //if (filter.SartDate.HasValue)
            //{
            //    predicate = predicate.And(x => x.SartDate.Date >= filter.SartDate.Value.Date);
            //}
            //if (filter.EndDate.HasValue)
            //{
            //    predicate = predicate.And(x => x.EndDate<= filter.EndDate.Value.Date);
            //}
            //if (!string.IsNullOrWhiteSpace(filter.Code))
            //{
            //    predicate = predicate.And(x => x.Code.Contains(filter.Code));
            //}

            return predicate;
        }
        static Expression<Func<Entities.Tracker.TripGeo, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.TripGeo>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                //predicate = predicate.And(b => b.Code.Contains(filter.SearchCriteria));
                //  predicate = predicate.Or(b => b.Name.Contains(filter.SearchCriteria));
            }
            return predicate;
        }

        public async Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            var idsList = ids.ToList();
            var entitiesToDelete = await UnitOfWork.Repository.FindAsync(d => idsList.Contains(d.Id), cancellationToken: cancellationToken);

            UnitOfWork.Repository.RemoveRange(entitiesToDelete);

            var rows = await UnitOfWork.SaveChangesAsync(cancellationToken);

            return ResponseResult.PostResult(result: rows, status: HttpStatusCode.NoContent, message: MessagesConstants.DeleteSuccess);
        }
    }
}
