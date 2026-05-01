using LinqKit;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Helpers;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.City;
using SonoTracker.Common.DTO.Lookup.InspectionType;
using SonoTracker.Common.DTO.Lookup.InspectionType.Parameters;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.LookUp.InspectionType
{
    public class InspectionTypeService(IServiceBaseParameter<Entities.Lookups.InspectionType> businessBaseParameter) : BaseService<Entities.Lookups.InspectionType, AddInspectionTypeDto, EditInspectionTypeDto, InspectionTypeDto, string, string>(businessBaseParameter), IInspectionTypeService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Domain.Entities.Lookups.InspectionType, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            IEnumerable<Domain.Entities.Lookups.InspectionType> entities = await UnitOfWork.Repository.GetAllAsync(disableTracking: disableTracking, cancellationToken: cancellationToken);

            var governorateId = IsSuperAdmin() ? null : GetGovernorateIdFromClaims();
            var filtered = IsSuperAdmin()
                ? (entities ?? Enumerable.Empty<Domain.Entities.Lookups.InspectionType>())
                : (entities?.Where(e =>
                    !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.GovernorateId == governorateId)) ?? Enumerable.Empty<Domain.Entities.Lookups.InspectionType>());
            IEnumerable<InspectionTypeDto> mapped = Mapper.Map<IEnumerable<Domain.Entities.Lookups.InspectionType>, IEnumerable<InspectionTypeDto>>(filtered);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<InspectionTypeFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var inspectionFilter = filter?.Filter ?? new InspectionTypeFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            if (!isSuperAdmin)
                inspectionFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            (int Count, IEnumerable<Entities.Lookups.InspectionType> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: PredicateBuilderFunction(inspectionFilter, governorateId),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            var filteredResult = isSuperAdmin
                ? (Result ?? Enumerable.Empty<Entities.Lookups.InspectionType>())
                : (Result?.Where(x => x.IsDeleted != true) ?? Enumerable.Empty<Entities.Lookups.InspectionType>());
            var data = Mapper.Map<IEnumerable<Entities.Lookups.InspectionType>, IEnumerable<InspectionTypeDto>>(filteredResult);

            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.InspectionType>, IEnumerable<InspectionTypeDto>>(query.Result.Where(x => x.IsDeleted != true));

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        static Expression<Func<Entities.Lookups.InspectionType, bool>> PredicateBuilderFunction(InspectionTypeFilter filter, string governorateId)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.InspectionType>(x => x.IsDeleted == filter.IsDeleted);
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
        static Expression<Func<Entities.Lookups.InspectionType, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.InspectionType>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.NameEn.Contains(filter.SearchCriteria));
            }
            return predicate;
        }
        public override async Task<IFinalResult> AddAsync(AddInspectionTypeDto model, CancellationToken cancellationToken = default)
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

                var entity = Mapper.Map<Entities.Lookups.InspectionType>(model);
                entity.GovernorateId = GetGovernorateIdFromClaims();

                SetEntityCreatedBaseProperties(entity);

                IFinalResult lastEntity = await GetLastRecordAsync(cancellationToken);

                if (lastEntity.Data != null)
                {
                    if (lastEntity.Data is InspectionTypeDto inspectionTypeDto)
                    {
                        if (int.TryParse(inspectionTypeDto.Code.AsSpan(inspectionTypeDto.Code.Length - 2), out int num))
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

                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.AddError);

                return ResponseResult.PostResult(result: entity.Id, status: HttpStatusCode.Created, exception: null,
                    message: MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.AddError + ex.Message);
            }
        }
        public override async Task<IFinalResult> UpdateAsync(AddInspectionTypeDto model, CancellationToken cancellationToken = default)
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

                Domain.Entities.Lookups.InspectionType entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                var entity = Mapper.Map(model, entityToUpdate);
                entity.GovernorateId = GetGovernorateIdFromClaims();

                SetEntityModifiedBaseProperties(entity);

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        entity.IsDeleted = false;
                }

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
