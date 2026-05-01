using LinqKit;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Helpers;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.MaintenanceType;
using SonoTracker.Common.DTO.Lookup.MaintenanceType.Parameters;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Lookup.MaintenanceType
{
    public class MaintenanceTypeService(IServiceBaseParameter<Entities.Lookups.MaintenanceType> businessBaseParameter) : BaseService<Entities.Lookups.MaintenanceType, AddMaintenanceTypeDto, EditMaintenanceTypeDto, MaintenanceTypeDto, string, string>(businessBaseParameter), IMaintenanceTypeService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Domain.Entities.Lookups.MaintenanceType, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            IEnumerable<Domain.Entities.Lookups.MaintenanceType> entities = await UnitOfWork.Repository.GetAllAsync(disableTracking: disableTracking, cancellationToken: cancellationToken);

            var governorateId = IsSuperAdmin() ? null : GetGovernorateIdFromClaims();
            var filtered = IsSuperAdmin()
                ? (entities ?? Enumerable.Empty<Domain.Entities.Lookups.MaintenanceType>())
                : (entities?.Where(e =>
                    !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.GovernorateId == governorateId)) ?? Enumerable.Empty<Domain.Entities.Lookups.MaintenanceType>());
            IEnumerable<MaintenanceTypeDto> mapped = Mapper.Map<IEnumerable<Domain.Entities.Lookups.MaintenanceType>, IEnumerable<MaintenanceTypeDto>>(filtered);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<MaintenanceTypeFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var maintenanceFilter = filter?.Filter ?? new MaintenanceTypeFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            if (!isSuperAdmin)
                maintenanceFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            (int Count, IEnumerable<Entities.Lookups.MaintenanceType> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: PredicateBuilderFunction(maintenanceFilter, governorateId),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            var filteredResult = isSuperAdmin
                ? (Result ?? Enumerable.Empty<Entities.Lookups.MaintenanceType>())
                : (Result?.Where(x => x.IsDeleted != true) ?? Enumerable.Empty<Entities.Lookups.MaintenanceType>());
            var data = Mapper.Map<IEnumerable<Entities.Lookups.MaintenanceType>, IEnumerable<MaintenanceTypeDto>>(filteredResult);

            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.MaintenanceType>, IEnumerable<MaintenanceTypeDto>>(query.Result.Where(x => x.IsDeleted != true));

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        static Expression<Func<Entities.Lookups.MaintenanceType, bool>> PredicateBuilderFunction(MaintenanceTypeFilter filter, string governorateId)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.MaintenanceType>(x => x.IsDeleted == filter.IsDeleted);
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
                predicate = predicate.And(x => x.GovernorateId == governorateId);
            }

            return predicate;
        }
        static Expression<Func<Entities.Lookups.MaintenanceType, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.MaintenanceType>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.NameEn.Contains(filter.SearchCriteria));
            }
            return predicate;
        }
        public override async Task<IFinalResult> AddAsync(AddMaintenanceTypeDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var isSuperAdmin = IsSuperAdmin();
                var govId = GetGovernorateIdFromClaims();
                var existingForDup = isSuperAdmin || string.IsNullOrWhiteSpace(govId)
                    ? await UnitOfWork.Repository.FindAsync(disableTracking: true, cancellationToken: cancellationToken)
                    : await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.GovernorateId == govId,
                        disableTracking: true,
                        cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.NameAr, x => x.NameEn, model.NameAr, model.NameEn))
                    return new ResponseResult().PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                var entity = Mapper.Map<Entities.Lookups.MaintenanceType>(model);
                entity.GovernorateId = GetGovernorateIdFromClaims();

                IFinalResult lastEntity = await GetLastRecordAsync(cancellationToken);

                if (lastEntity.Data != null)
                {
                    if (lastEntity.Data is MaintenanceTypeDto maintenanceTypeDto)
                    {
                        if (int.TryParse(maintenanceTypeDto.Code.AsSpan(maintenanceTypeDto.Code.Length - 2), out int num))
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

        public override async Task<IFinalResult> UpdateAsync(AddMaintenanceTypeDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var isSuperAdmin = IsSuperAdmin();
                var govId = GetGovernorateIdFromClaims();
                var existingForDup = isSuperAdmin || string.IsNullOrWhiteSpace(govId)
                    ? await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.Id != model.Id,
                        disableTracking: true,
                        cancellationToken: cancellationToken)
                    : await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.GovernorateId == govId && x.Id != model.Id,
                        disableTracking: true,
                        cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.NameAr, x => x.NameEn, model.NameAr, model.NameEn))
                    return new ResponseResult().PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                Domain.Entities.Lookups.MaintenanceType entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                var entity = Mapper.Map(model, entityToUpdate);
                entity.GovernorateId = GetGovernorateIdFromClaims();

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
