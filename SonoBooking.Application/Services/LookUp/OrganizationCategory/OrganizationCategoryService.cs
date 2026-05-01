using LinqKit;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Helpers;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.OrganizationCategory;
using SonoTracker.Common.DTO.Lookup.OrganizationCategory.Parameters;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.LookUp.OrganizationCategory
{
    public class OrganizationCategoryService(IServiceBaseParameter<Entities.Lookups.OrganizationCategory> businessBaseParameter) : BaseService<Entities.Lookups.OrganizationCategory, AddOrganizationCategoryDto, EditOrganizationCategoryDto, OrganizationCategoryDto, string, string>(businessBaseParameter), IOrganizationCategoryService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Domain.Entities.Lookups.OrganizationCategory, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            IEnumerable<Entities.Lookups.OrganizationCategory> entities = await UnitOfWork.Repository.GetAllAsync(disableTracking: disableTracking, cancellationToken: cancellationToken);

            var governorateId = IsSuperAdmin() ? null : GetGovernorateIdFromClaims();
            var filtered = IsSuperAdmin()
                ? (entities ?? Enumerable.Empty<Entities.Lookups.OrganizationCategory>())
                : (entities?.Where(e =>
                    !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.GovernorateId == governorateId)) ?? Enumerable.Empty<Entities.Lookups.OrganizationCategory>());
            IEnumerable<OrganizationCategoryDto> mapped = Mapper.Map<IEnumerable<Entities.Lookups.OrganizationCategory>, IEnumerable<OrganizationCategoryDto>>(filtered);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<OrganizationCategoryFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var organizationCategoryFilter = filter?.Filter ?? new OrganizationCategoryFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            if (!isSuperAdmin)
                organizationCategoryFilter.IsDeleted = false;

            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;

            (int Count, IEnumerable<Entities.Lookups.OrganizationCategory> Result) = await UnitOfWork.Repository
                .FindPagedAsync(
                    predicate: PredicateBuilderFunction(organizationCategoryFilter, governorateId),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    cancellationToken: cancellationToken);

            var filteredResult = isSuperAdmin
                ? (Result ?? Enumerable.Empty<Entities.Lookups.OrganizationCategory>())
                : (Result?.Where(x => x.IsDeleted != true) ?? Enumerable.Empty<Entities.Lookups.OrganizationCategory>());
            var data = Mapper.Map<IEnumerable<Entities.Lookups.OrganizationCategory>, IEnumerable<OrganizationCategoryDto>>(filteredResult);

            return new PagingResult(pageNumber: filter.PageNumber, pageSize: filter.PageSize, totalCount: Count, result: data,
                status: HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;
            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.OrganizationCategory>, IEnumerable<OrganizationCategoryDto>>(query.Result.Where(x => x.IsDeleted != true));

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<Entities.Lookups.OrganizationCategory, bool>> PredicateBuilderFunction(OrganizationCategoryFilter filter, string governorateId)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.OrganizationCategory>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.NameAr))
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            if (!string.IsNullOrWhiteSpace(filter.NameEn))
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));
            if (!string.IsNullOrWhiteSpace(governorateId))
                predicate = predicate.And(x => x.GovernorateId == governorateId);
            return predicate;
        }

        static Expression<Func<Entities.Lookups.OrganizationCategory, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.OrganizationCategory>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.NameEn.Contains(filter.SearchCriteria));
            }
            return predicate;
        }

        public override async Task<IFinalResult> AddAsync(AddOrganizationCategoryDto model, CancellationToken cancellationToken = default)
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

                var entity = Mapper.Map<Entities.Lookups.OrganizationCategory>(model);
                entity.GovernorateId = GetGovernorateIdFromClaims();

                IFinalResult lastEntity = await GetLastRecordAsync(cancellationToken);

                if (lastEntity.Data != null)
                {
                    if (lastEntity.Data is OrganizationCategoryDto dto)
                    {
                        if (int.TryParse(dto.Code.AsSpan(dto.Code.Length - 2), out int num))
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

        public override async Task<IFinalResult> UpdateAsync(AddOrganizationCategoryDto model, CancellationToken cancellationToken = default)
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

                Entities.Lookups.OrganizationCategory entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

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
