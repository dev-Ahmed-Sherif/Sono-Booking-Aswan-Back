using LinqKit;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Common.DTO.Tracker.FloatingUnitOrganization.Parameters;
using SonoTracker.Common.DTO.Tracker.FloatingUnitOrganization;
using Microsoft.EntityFrameworkCore;

namespace SonoTracker.Application.Services.Tracker.FloatingUnitOrganization
{
    public class FloatingUnitOrganizationService(IServiceBaseParameter<Entities.Tracker.FloatingUnitOrganization> businessBaseParameter) : BaseService<Entities.Tracker.FloatingUnitOrganization, AddFloatingUnitOrganizationDto, EditFloatingUnitOrganizationDto, FloatingUnitOrganizationDto, string, string>(businessBaseParameter), IFloatingUnitOrganizationService
    {
        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src
                .Include(t => t.FloatingUnit)
                .Include(t => t.Organization), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Domain.Entities.Tracker.FloatingUnitOrganization, EditFloatingUnitOrganizationDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src
                .Include(t => t.FloatingUnit)
                .Include(t => t.Organization), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Domain.Entities.Tracker.FloatingUnitOrganization, FloatingUnitOrganizationDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }
       
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.FloatingUnitOrganization, bool>> predicate = null, CancellationToken cancellationToken = default)
            => await GetAllAsync(floatingUnitId: null, cancellationToken);

        public async Task<IFinalResult> GetAllAsync(string floatingUnitId, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var filter = new FloatingUnitOrganizationFilter
            {
                FloatingUnitId = floatingUnitId,
                IsDeleted = false
            };
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            var entity = await UnitOfWork.Repository.FindAsync(
                predicate: PredicateBuilderFunction(filter, governorateId, includeDeleted: isSuperAdmin),
                include: src => src
                    .Include(t => t.FloatingUnit)
                    .Include(t => t.Organization),
                cancellationToken: cancellationToken);
            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.FloatingUnitOrganization>, IEnumerable<FloatingUnitOrganizationDto>>(entity);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<FloatingUnitOrganizationFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var floatingUnitOrganizationFilter = filter?.Filter ?? new FloatingUnitOrganizationFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            if (!isSuperAdmin)
                floatingUnitOrganizationFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(
                predicate: PredicateBuilderFunction(floatingUnitOrganizationFilter, governorateId, includeDeleted: false),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                include: src => src
                    .Include(t => t.FloatingUnit)
                    .Include(t => t.Organization),
                cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.FloatingUnitOrganization>, IEnumerable<FloatingUnitOrganizationDto>>(query.Item2);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<Entities.Tracker.FloatingUnitOrganization, bool>> PredicateBuilderFunction(
            FloatingUnitOrganizationFilter filter,
            string governorateId = null,
            bool includeDeleted = false)
        {
            var predicate = includeDeleted
                ? PredicateBuilder.New<Entities.Tracker.FloatingUnitOrganization>(true)
                : PredicateBuilder.New<Entities.Tracker.FloatingUnitOrganization>(x => x.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.OrganizationId))
            {
                predicate = predicate.And(x => x.OrganizationId == filter.OrganizationId);
            }
            if (filter.OrganizationType.HasValue)
            {
                predicate = predicate.And(x => x.Organization != null && x.Organization.OrganizationType == filter.OrganizationType);
            }
            if (!string.IsNullOrEmpty(filter.FloatingUnitId))
            {
                predicate = predicate.And(x => x.FloatingUnitId == filter.FloatingUnitId);
            }
            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x =>
                    (x.FloatingUnit != null && x.FloatingUnit.GovernorateId == governorateId) ||
                    (x.Organization != null && x.Organization.GovernorateId == governorateId));
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
