using LinqKit;
using Microsoft.EntityFrameworkCore;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Helpers;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.Town;
using SonoTracker.Common.DTO.Lookup.Town.Parameters;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Lookup.Town
{
    public class TownService(IServiceBaseParameter<Domain.Entities.Lookups.Town> businessBaseParameter) : BaseService<Domain.Entities.Lookups.Town, AddTownDto, EditTownDto, TownDto, string, string>(businessBaseParameter), ITownService
    {
        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id.Equals(id),
                include: src => src
                .Include(t => t.City)
                .ThenInclude(x => x.Governorate), cancellationToken: cancellationToken);

            var mapped = Mapper.Map<Domain.Entities.Lookups.Town, EditTownDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id.Equals(id),
                include: src => src
                .Include(t => t.City)
                .ThenInclude(x => x.Governorate), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Domain.Entities.Lookups.Town, TownDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Lookups.Town, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var entities = await UnitOfWork.Repository.GetAllAsync(include: src => src
                .Include(t => t.City)
                .ThenInclude(x => x.Governorate),
                disableTracking: disableTracking,
                cancellationToken: cancellationToken);

            var governorateId = IsSuperAdmin() ? null : GetGovernorateIdFromClaims();
            var filtered = IsSuperAdmin()
                ? (entities ?? Enumerable.Empty<Domain.Entities.Lookups.Town>())
                : (entities?.Where(e => !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.City?.GovernorateId == governorateId)) ?? Enumerable.Empty<Domain.Entities.Lookups.Town>());
            var mapped = Mapper.Map<IEnumerable<Domain.Entities.Lookups.Town>, IEnumerable<TownDto>>(filtered);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<TownFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var townFilter = filter?.Filter ?? new TownFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            if (!isSuperAdmin)
                townFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            (int Count, IEnumerable<Domain.Entities.Lookups.Town> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: PredicateBuilderFunction(townFilter, governorateId),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                include: src => src
                    .Include(t => t.City)
                    .ThenInclude(x => x.Governorate),
                cancellationToken: cancellationToken);

            var filteredResult = isSuperAdmin
                ? (Result ?? Enumerable.Empty<Domain.Entities.Lookups.Town>())
                : (Result?.Where(x => x.IsDeleted != true) ?? Enumerable.Empty<Domain.Entities.Lookups.Town>());
            var data = Mapper.Map<IEnumerable<Domain.Entities.Lookups.Town>, IEnumerable<TownDto>>(filteredResult);

            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            if (!isSuperAdmin)
                predicate = predicate.And(x => x.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(governorateId))
                predicate = predicate.And(x => x.City.GovernorateId == governorateId);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Domain.Entities.Lookups.Town>, IEnumerable<TownDto>>(query.Result);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        static Expression<Func<Domain.Entities.Lookups.Town, bool>> PredicateBuilderFunction(TownFilter filter, string governorateId)
        {
            var predicate = PredicateBuilder.New<Domain.Entities.Lookups.Town>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.NameAr))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            }
            if (!string.IsNullOrWhiteSpace(filter.NameEn))
            {
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));
            }

            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x => x.City.GovernorateId == governorateId);
            }

            return predicate;
        }
        static Expression<Func<Domain.Entities.Lookups.Town, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Domain.Entities.Lookups.Town>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.NameEn.Contains(filter.SearchCriteria));
            }
            return predicate;
        }
        public override async Task<IFinalResult> AddAsync(AddTownDto model, CancellationToken cancellationToken = default)
        {
            try
            {

                var existingForDup = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.CityId == model.CityId,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.NameAr, x => x.NameEn, model.NameAr, model.NameEn))
                    return new ResponseResult().PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                var entity = Mapper.Map<Entities.Lookups.Town>(model);

                IFinalResult lastEntity = await GetLastRecordAsync(cancellationToken);

                if (lastEntity.Data != null)
                {
                    if (lastEntity.Data is TownDto townDto)
                    {
                        if (int.TryParse(townDto.Code.AsSpan(townDto.Code.Length - 2), out int num))
                        {
                            ++num;
                            entity.Code = num.ToString("D2");
                        }
                    }
                }
                else
                {
                    entity.Code = "01";
                }

                SetEntityCreatedBaseProperties(entity);
                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.AddError);

                return ResponseResult.PostResult(result: entity.Id, status: HttpStatusCode.Created, exception: null,
                    message: MessagesConstants.AddSuccess);
            }
            catch (Exception e)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: e,
                    message: MessagesConstants.AddError + e.Message);
            }

        }

        public override async Task<IFinalResult> UpdateAsync(AddTownDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingForDup = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.CityId == model.CityId && x.Id != model.Id,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.NameAr, x => x.NameEn, model.NameAr, model.NameEn))
                    return new ResponseResult().PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                Domain.Entities.Lookups.Town entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                var entity = Mapper.Map(model, entityToUpdate);

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        entity.IsDeleted = false;
                }

                SetEntityModifiedBaseProperties(entity);
                UnitOfWork.Repository.Update(entityToUpdate, entity);

                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.UpdateError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.OK, exception: null,
                    message: MessagesConstants.UpdateSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.UpdateError + ex.Message);
            }
        }
    }
}
