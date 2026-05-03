using LinqKit;
using Microsoft.EntityFrameworkCore;
using SonoBooking.Common.DTO.Tracker.AccidentOrganization.Parameters;
using SonoTracker.Application.Services.Base;
using SonoTracker.Application.Services.Tracker.FloatingUnitOrganization;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.AccidentOrganization;
using SonoTracker.Common.DTO.Tracker.FloatingUnitOrganization;
using SonoTracker.Common.DTO.Tracker.FloatingUnitOrganization.Parameters;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.AccidentOrganization
{
    public class AccidentOrganizationService(IServiceBaseParameter<Entities.Tracker.AccidentOrganization> businessBaseParameter) : BaseService<Entities.Tracker.AccidentOrganization, AddAccidentOrganizationDto, EditAccidentOrganizationDto, AccidentOrganizationDto, string, string>(businessBaseParameter), IAccidentOrganizationService
    {
        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src
                .Include(t => t.Accident)
                .Include(t => t.Organization), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.AccidentOrganization, EditAccidentOrganizationDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src
                .Include(t => t.Accident)
                .Include(t => t.Organization), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.AccidentOrganization, AccidentOrganizationDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }
       
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.AccidentOrganization, bool>> predicate = null, CancellationToken cancellationToken = default)
            => await GetAllAsync(accidentId: null, cancellationToken);

        public async Task<IFinalResult> GetAllAsync(string accidentId, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var filter = new AccidentOrganizationFilter
            {
                AccidentId = accidentId,
                IsDeleted = false
            };
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            var entity = await UnitOfWork.Repository.FindAsync(
                predicate: PredicateBuilderFunction(filter, governorateId, includeDeleted: isSuperAdmin),
                include: src => src
                    .Include(t => t.Accident)
                    .Include(t => t.Organization),
                cancellationToken: cancellationToken);
            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.AccidentOrganization>, IEnumerable<AccidentOrganizationDto>>(entity);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<AccidentOrganizationFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var floatingUnitOrganizationFilter = filter?.Filter ?? new AccidentOrganizationFilter();
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
                    .Include(t => t.Accident)
                    .Include(t => t.Organization),
                cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.AccidentOrganization>, IEnumerable<AccidentOrganizationDto>>(query.Item2);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<Entities.Tracker.AccidentOrganization, bool>> PredicateBuilderFunction(
            AccidentOrganizationFilter filter,
            string governorateId = null,
            bool includeDeleted = false)
        {
            var predicate = includeDeleted
                ? PredicateBuilder.New<Entities.Tracker.AccidentOrganization>(true)
                : PredicateBuilder.New<Entities.Tracker.AccidentOrganization>(x => x.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.OrganizationId))
            {
                predicate = predicate.And(x => x.OrganizationId == filter.OrganizationId);
            }
            if (!string.IsNullOrEmpty(filter.AccidentId))
            {
                predicate = predicate.And(x => x.AccidentId == filter.AccidentId);
            }
            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x =>
                    (x.Accident != null && x.Accident.GovernorateId == governorateId) ||
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
