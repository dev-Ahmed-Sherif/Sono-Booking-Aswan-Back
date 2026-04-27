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
using SonoTracker.Common.DTO.Tracker.TripMarina.Parameters;
using SonoTracker.Common.DTO.Tracker.TripMarina;

namespace SonoTracker.Application.Services.Tracker.TripMarina
{
    public class TripMarinaService : BaseService<Entities.Tracker.TripMarina, AddTripMarinaDto, EditTripMarinaDto, TripMarinaDto, string, string>, ITripMarinaService
    {

        public TripMarinaService(IServiceBaseParameter<Entities.Tracker.TripMarina> businessBaseParameter) : base(businessBaseParameter)
        {


        }
        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src
                .Include(t => t.TouristMarina)
               .Include(x => x.TripInformation), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.TripMarina, EditTripMarinaDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src
               .Include(t => t.TouristMarina)
              .Include(x => x.TripInformation)
              .ThenInclude(x => x.FloatingUnit), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.TripMarina, TripMarinaDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.TripMarina, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.GetAllAsync(include: src => src
               .Include(t => t.TouristMarina)
              .Include(x => x.TripInformation)
              .ThenInclude(x => x.FloatingUnit), cancellationToken: cancellationToken);
            var filteredEntities = IsSuperAdmin()
                ? entity
                : entity.Where(e => !e.IsDeleted);
            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.TripMarina>, IEnumerable<TripMarinaDto>>(filteredEntities);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<TripMarinaFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var marinaTripFilter = filter?.Filter ?? new TripMarinaFilter();
            if (!isSuperAdmin)
                marinaTripFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(marinaTripFilter),
                pageNumber: offset, pageSize: limit,
                filter.OrderByValue, include: src => src
                .Include(t => t.TouristMarina)
              .Include(x => x.TripInformation)
              .ThenInclude(x => x.FloatingUnit),
                cancellationToken: cancellationToken);

            var items = isSuperAdmin ? query.Item2 : query.Item2.Where(x => x.IsDeleted != true);
            var data = Mapper.Map<IEnumerable<Entities.Tracker.TripMarina>, IEnumerable<TripMarinaDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }


        static Expression<Func<Entities.Tracker.TripMarina, bool>> PredicateBuilderFunction(TripMarinaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.TripMarina>(x => x.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.TouristMarinaId))
            {
                predicate = predicate.And(x => x.TouristMarinaId == filter.TouristMarinaId);
            }
            if (!string.IsNullOrEmpty(filter.TripInformationId))
            {
                predicate = predicate.And(x => x.TripInformationId == filter.TripInformationId);
            }

            if (!string.IsNullOrEmpty(filter.FloatingUnitId))
            {
                predicate = predicate.And(x => x.TripInformation.FloatingUnitId == filter.FloatingUnitId);
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
