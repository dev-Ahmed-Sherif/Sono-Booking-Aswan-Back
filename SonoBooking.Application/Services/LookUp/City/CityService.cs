using FuzzySharp;
using LinqKit;
using SonoBooking.Application.Services.Base;
using SonoBooking.Application.Services.Lookup.City;
using SonoBooking.Application.Services.LookUp.Governorate;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.City;
using SonoBooking.Common.DTO.Lookup.City.Parameters;
using SonoBooking.Common.DTO.Lookup.Governorate;
using SonoBooking.Domain;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.LookUp.City
{
    public class CityService(
        IServiceBaseParameter<Entities.Lookups.City> businessBaseParameter,
        IGovernorateService governorateService)
        : BaseService<Entities.Lookups.City, AddCityDto, EditCityDto, CityDto, string, string>(businessBaseParameter), ICityService
    {
        private readonly IGovernorateService _governorateService = governorateService;
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Lookups.City, bool>> predicate = null, CancellationToken cancellationToken = default)
            => await GetAllAsync(governorateId: null, cancellationToken);

        public async Task<IFinalResult> GetAllAsync(string governorateId, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var governorateScope = governorateId;
            if (!isSuperAdmin && string.IsNullOrWhiteSpace(governorateScope))
                governorateScope = GetGovernorateIdFromClaims();

            var filter = new CityFilter
            {
                // Non-superadmin always gets non-deleted rows
                // (for superadmin, the predicate bypasses IsDeleted entirely)
                IsDeleted = false
            };

            var entity = await UnitOfWork.Repository.FindAsync(
                predicate: PredicateBuilderFunction(filter, includeDeleted: isSuperAdmin, governorateScope: governorateScope),
                cancellationToken: cancellationToken);
            var mapped = Mapper.Map<IEnumerable<Entities.Lookups.City>, IEnumerable<CityDto>>(entity);
            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<CityFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var cityFilter = filter?.Filter ?? new CityFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            if (!isSuperAdmin)
                cityFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            (int Count, IEnumerable<Entities.Lookups.City> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: PredicateBuilderFunction(cityFilter, includeDeleted: false, governorateScope: governorateId),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            var filteredResult = isSuperAdmin
                ? (Result ?? [])
                : (Result?.Where(x => x.IsDeleted != true) ?? Enumerable.Empty<Entities.Lookups.City>());
            var data = Mapper.Map<IEnumerable<Entities.Lookups.City>, IEnumerable<CityDto>>(filteredResult);

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
                predicate = predicate.And(x => x.GovernorateId == governorateId);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.City>, IEnumerable<CityDto>>(query.Result);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);

        }

        static Expression<Func<Entities.Lookups.City, bool>> PredicateBuilderFunction(CityFilter filter, bool includeDeleted, string governorateScope = null)
        {
            var predicate = includeDeleted
                ? PredicateBuilder.New<Entities.Lookups.City>(true)
                : PredicateBuilder.New<Entities.Lookups.City>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.NameAr))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            }
            if (!string.IsNullOrWhiteSpace(filter.NameEn))
            {
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));
            }
            var governorateId = !string.IsNullOrWhiteSpace(governorateScope) ? governorateScope : filter.GovernorateId;
            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x => x.GovernorateId == governorateId);
            }

            return predicate;
        }

        static Expression<Func<Entities.Lookups.City, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.City>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.NameEn.Contains(filter.SearchCriteria));

            }
            return predicate;
        }

        static string NormalizeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var parts = value.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', parts).ToLowerInvariant();
        }

        public override async Task<IFinalResult> AddAsync(AddCityDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingCities = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.GovernorateId == model.GovernateId,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                var normalizedNameAr = NormalizeText(model.NameAr);
                var normalizedNameEn = NormalizeText(model.NameEn);

                const int nameThreshold = 90;

                var nameArDuplicate = existingCities.Any(x =>
                    !string.IsNullOrWhiteSpace(x.NameAr) &&
                    !string.IsNullOrWhiteSpace(normalizedNameAr) &&
                    Fuzz.TokenSetRatio(normalizedNameAr, NormalizeText(x.NameAr)) >= nameThreshold);

                var nameEnDuplicate = existingCities.Any(x =>
                    !string.IsNullOrWhiteSpace(x.NameEn) &&
                    !string.IsNullOrWhiteSpace(normalizedNameEn) &&
                    Fuzz.TokenSetRatio(normalizedNameEn, NormalizeText(x.NameEn)) >= nameThreshold);

                if (nameArDuplicate || nameEnDuplicate)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                var entity = Mapper.Map<AddCityDto, Entities.Lookups.City>(model);
                entity.GovernorateId = model.GovernateId;

                IFinalResult governorateResult = await _governorateService.GetByIdAsync(model.GovernateId, cancellationToken);
                if (governorateResult?.Data is not GovernorateDto governorateDto || string.IsNullOrWhiteSpace(governorateDto.Code))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.NotFound);

                var governorateCode = governorateDto.Code.Trim();

                var lastCity = existingCities
                    .OrderByDescending(x => x.Code)
                    .FirstOrDefault();

                if (lastCity != null)
                {
                    if (!string.IsNullOrEmpty(lastCity.Code) && lastCity.Code.Length >= 2 &&
                        int.TryParse(lastCity.Code.AsSpan(lastCity.Code.Length - 2), out int num))
                    {
                        ++num;
                        var prefix = lastCity.Code.Length > 2
                            ? lastCity.Code[..^2]
                            : governorateCode;
                        entity.Code = prefix + num.ToString("D2");
                    }
                }
                else
                {
                    entity.Code = governorateCode + "01";
                }
                SetEntityCreatedBaseProperties(entity);
                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.AddError);

                return ResponseResult.PostResult(result: entity.Id, status: HttpStatusCode.Created, exception: null,
                    message: MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.AddError + ex.Message);
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddCityDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                Entities.Lookups.City entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                var entity = Mapper.Map(model, entityToUpdate);
                entity.GovernorateId = GetGovernorateIdFromClaims();

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        entity.IsDeleted = false;
                }

                var existingCities = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.GovernorateId == model.GovernateId && x.Id != model.Id,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                var normalizedNameAr = NormalizeText(model.NameAr);
                var normalizedNameEn = NormalizeText(model.NameEn);

                const int nameThreshold = 90;

                var nameArDuplicate = existingCities.Any(x =>
                    !string.IsNullOrWhiteSpace(x.NameAr) &&
                    !string.IsNullOrWhiteSpace(normalizedNameAr) &&
                    Fuzz.TokenSetRatio(normalizedNameAr, NormalizeText(x.NameAr)) >= nameThreshold);

                var nameEnDuplicate = existingCities.Any(x =>
                    !string.IsNullOrWhiteSpace(x.NameEn) &&
                    !string.IsNullOrWhiteSpace(normalizedNameEn) &&
                    Fuzz.TokenSetRatio(normalizedNameEn, NormalizeText(x.NameEn)) >= nameThreshold);

                if (nameArDuplicate || nameEnDuplicate)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                UnitOfWork.Repository.Update(entityToUpdate, entity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.UpdateError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                    message: MessagesConstants.UpdateSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.UpdateError + ex.Message);
            }
        }
    }
}

