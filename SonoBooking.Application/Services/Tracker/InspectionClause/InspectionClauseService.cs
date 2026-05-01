using LinqKit;
using Microsoft.EntityFrameworkCore;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.InspectionClause;
using SonoTracker.Common.DTO.Tracker.InspectionClause.Parameters;
using SonoTracker.Common.Helpers;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.InspectionClause
{
    public class InspectionClauseService(IServiceBaseParameter<Domain.Entities.Tracker.InspectionClause> businessBaseParameter) : BaseService<Domain.Entities.Tracker.InspectionClause, AddInspectionClauseDto, EditInspectionClauseDto, InspectionClauseDto, string, string>(businessBaseParameter), IInspectionClauseService
    {
        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src
                    .Include(c => c.InspectionType)
                    .Include(c => c.Parent), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Domain.Entities.Tracker.InspectionClause, InspectionClauseDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr, include: src => src
                    .Include(c => c.InspectionType)
                    .Include(c => c.Parent), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Domain.Entities.Tracker.InspectionClause, EditInspectionClauseDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK, exception: null, message: HttpStatusCode.OK.ToString());
        }

        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Domain.Entities.Tracker.InspectionClause, bool>> predicate = null, CancellationToken cancellationToken = default)
            => await GetAllAsync(inspectionTypeId: null, cancellationToken);

        public async Task<IFinalResult> GetAllAsync(string inspectionTypeId, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            var entities = await UnitOfWork.Repository.GetAllAsync(
                include: src => src
                    .Include(c => c.InspectionType)
                    .Include(c => c.Parent),
                cancellationToken: cancellationToken);

            var filteredEntities = isSuperAdmin
                ? entities
                : entities.Where(e => !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.GovernorateId == governorateId));

            if (!string.IsNullOrEmpty(inspectionTypeId))
                filteredEntities = filteredEntities.Where(e => e.InspectionTypeId == inspectionTypeId);

            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.InspectionClause>, IEnumerable<InspectionClauseDto>>(filteredEntities);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK, exception: null, message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<InspectionClauseFilter> filter, CancellationToken cancellationToken = default)
        {
            var clauseFilter = filter?.Filter ?? new InspectionClauseFilter();
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(
                predicate: PredicateBuilderFunction(clauseFilter),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                include: src => src
                    .Include(c => c.InspectionType)
                    .Include(c => c.Parent),
                cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Domain.Entities.Tracker.InspectionClause>, IEnumerable<InspectionClauseDto>>(query.Item2);
            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public override async Task<IFinalResult> AddAsync(AddInspectionClauseDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var isSuperAdmin = IsSuperAdmin();
                var govId = GetGovernorateIdFromClaims();

                var existingForDup = isSuperAdmin || string.IsNullOrWhiteSpace(govId)
                    ? await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.IsDeleted != true && x.InspectionTypeId == dto.InspectionTypeId,
                        disableTracking: true,
                        cancellationToken: cancellationToken)
                    : await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.GovernorateId == govId && x.IsDeleted != true && x.InspectionTypeId == dto.InspectionTypeId,
                        disableTracking: true,
                        cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.Code, x => x.Name, dto.Code, dto.Name))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                if (!string.IsNullOrWhiteSpace(dto.Code) && LookupDuplicateGuard.HasFuzzyCodeDuplicate(existingForDup, x => x.Code, dto.Code))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                var mapped = Mapper.Map<Entities.Tracker.InspectionClause>(dto);
                mapped.GovernorateId = govId;
                SetEntityCreatedBaseProperties(mapped);

                UnitOfWork.Repository.Add(mapped);
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.AddError);

                return ResponseResult.PostResult(result: mapped.Id, HttpStatusCode.Created, null, MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.AddError + ex.Message);
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddInspectionClauseDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var isSuperAdmin = IsSuperAdmin();
                var govId = GetGovernorateIdFromClaims();

                var existingForDup = isSuperAdmin || string.IsNullOrWhiteSpace(govId)
                    ? await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.Id != dto.Id && x.IsDeleted != true,
                        disableTracking: true,
                        cancellationToken: cancellationToken)
                    : await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.GovernorateId == govId && x.Id != dto.Id && x.IsDeleted != true,
                        disableTracking: true,
                        cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.Code, x => x.Name, dto.Code, dto.Name))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                if (!string.IsNullOrWhiteSpace(dto.Code) && LookupDuplicateGuard.HasFuzzyCodeDuplicate(existingForDup, x => x.Code, dto.Code))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                var entityToUpdate = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id == dto.Id,
                    include: src => src
                        .Include(c => c.InspectionType)
                        .Include(c => c.Parent),
                    cancellationToken: cancellationToken);

                var newEntity = Mapper.Map(dto, entityToUpdate);
                newEntity.GovernorateId = GetGovernorateIdFromClaims();
                SetEntityModifiedBaseProperties(newEntity);

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        newEntity.IsDeleted = false;
                }

                UnitOfWork.Repository.Update(entityToUpdate, newEntity);
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.UpdateError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                    message: MessagesConstants.UpdateSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.UpdateError + ex.Message);
            }
        }

        public override async Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entityToDelete = await UnitOfWork.Repository.GetAsync(cancellationToken, id);
                UnitOfWork.Repository.Remove(entityToDelete);
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);
                if (affectedRows > 0)
                    Result = ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, message: MessagesConstants.DeleteSuccess);
                return Result;
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.DeleteError + ex.Message);
            }
        }

        public async Task<IFinalResult> DeleteRangeAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            var idsList = ids.ToList();
            var entitiesToDelete = await UnitOfWork.Repository.FindAsync(d => idsList.Contains(d.Id), cancellationToken: cancellationToken);
            UnitOfWork.Repository.RemoveRange(entitiesToDelete);
            var rows = await UnitOfWork.SaveChangesAsync(cancellationToken);
            return ResponseResult.PostResult(result: rows, status: HttpStatusCode.NoContent, message: MessagesConstants.DeleteSuccess);
        }

        static Expression<Func<Domain.Entities.Tracker.InspectionClause, bool>> PredicateBuilderFunction(InspectionClauseFilter filter)
        {
            var predicate = PredicateBuilder.New<Domain.Entities.Tracker.InspectionClause>(true);

            if (!string.IsNullOrWhiteSpace(filter.Code))
                predicate = predicate.And(x => x.Code.Contains(filter.Code));

            if (!string.IsNullOrWhiteSpace(filter.Name))
                predicate = predicate.And(x => x.Name.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.ParentId))
                predicate = predicate.And(x => x.ParentId == filter.ParentId);

            if (!string.IsNullOrEmpty(filter.InspectionTypeId))
                predicate = predicate.And(x => x.InspectionTypeId == filter.InspectionTypeId);

            return predicate;
        }
    }
}
