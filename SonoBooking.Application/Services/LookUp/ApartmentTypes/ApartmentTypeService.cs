using LinqKit;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Helpers;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Lookup.ApartmentType;
using SonoBooking.Common.DTO.Lookup.ApartmentType.Parameters;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using SonoBooking.Domain.Entities.Lookups;
using SonoBooking.Domain;
namespace SonoBooking.Application.Services.LookUp.ApartmentTypes
{
    public class ApartmentTypeService(IServiceBaseParameter<ApartmentType> businessBaseParameter) : BaseService<ApartmentType, AddApartmentTypeDto, EditApartmentTypeDto, ApartmentTypeDto, string, string>(businessBaseParameter), IApartmentTypeService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<ApartmentType, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            bool isSuperAdmin = IsSuperAdmin();

            IEnumerable<ApartmentType> entities = await UnitOfWork.Repository.FindAsync(
                predicate: predicate,
                disableTracking: disableTracking,
                cancellationToken: cancellationToken);

            IEnumerable<ApartmentType> filteredEntities = isSuperAdmin
                ? entities
                : entities.Where(e => !e.IsDeleted);

            IEnumerable<ApartmentTypeDto> mapped =
                Mapper.Map<IEnumerable<ApartmentType>, IEnumerable<ApartmentTypeDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<ApartmentTypeFilter> filter, CancellationToken cancellationToken = default)
        {
            bool isSuperAdmin = IsSuperAdmin();

            string governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            int limit = filter.PageSize;

            int offset = --filter.PageNumber * filter.PageSize;

            ApartmentTypeFilter accidentFilter = filter?.Filter ?? new ApartmentTypeFilter();

            if (!isSuperAdmin)
                accidentFilter.IsDeleted = false;

            (int Count, IEnumerable<ApartmentType> Result) =
                 await UnitOfWork.Repository.FindPagedAsync(
                 predicate: PredicateBuilderFunction(accidentFilter, governorateId),
                 pageNumber: offset,
                 pageSize: limit,
                 filter.OrderByValue,
                 cancellationToken: cancellationToken);

            IEnumerable<ApartmentType> filteredResult = isSuperAdmin ? (Result ?? []) : (Result?.Where(x => !x.IsDeleted) ?? []);

            IEnumerable<ApartmentTypeDto> data = Mapper.Map<IEnumerable<ApartmentType>, IEnumerable<ApartmentTypeDto>>(filteredResult);

            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            int limit = filter.PageSize;

            int offset = --filter.PageNumber * filter.PageSize;

            Expression<Func<ApartmentType, bool>> predicate = DropDownPredicateBuilderFunction(filter.Filter);

            (int Count, IEnumerable<ApartmentType> Result) query =
                await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            IEnumerable<ApartmentTypeDto> data = Mapper.Map<IEnumerable<ApartmentType>, IEnumerable<ApartmentTypeDto>>(query.Result.Where(x => x.IsDeleted != true));

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        static Expression<Func<ApartmentType, bool>> PredicateBuilderFunction(ApartmentTypeFilter filter, string governorateId)
        {
            var predicate = PredicateBuilder.New<ApartmentType>(x => x.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrWhiteSpace(filter.NameAr))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.NameAr));
            }
            if (!string.IsNullOrWhiteSpace(filter.NameEn))
            {
                predicate = predicate.And(x => x.NameEn.Contains(filter.NameEn));
            }

            return predicate;
        }
        static Expression<Func<ApartmentType, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<ApartmentType>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.NameEn.Contains(filter.SearchCriteria));
            }
            return predicate;
        }
        public override async Task<IFinalResult> AddAsync(AddApartmentTypeDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                bool isSuperAdmin = IsSuperAdmin();

                IEnumerable<ApartmentType> existingForDup =
                    await UnitOfWork.Repository.FindAsync(disableTracking: true, cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.NameAr, x => x.NameEn, model.NameAr, model.NameEn))
                    return new ResponseResult().PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                ApartmentType entity = Mapper.Map<ApartmentType>(model);

                SetEntityCreatedBaseProperties(entity);

                IFinalResult lastEntity = await GetLastRecordAsync(cancellationToken);

                if (lastEntity.Data != null)
                {
                    if (lastEntity.Data is ApartmentTypeDto accidentTypeDto)
                    {
                        if (int.TryParse(accidentTypeDto.Code.AsSpan(accidentTypeDto.Code.Length - 2), out int num))
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

                ApartmentType result = await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

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
                                                 message: MessagesConstants.AddError);
            }
        }
        public override async Task<IFinalResult> UpdateAsync(AddApartmentTypeDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                bool isSuperAdmin = IsSuperAdmin();

                string govId = GetGovernorateIdFromClaims();

                IEnumerable<ApartmentType> existingForDup = isSuperAdmin || string.IsNullOrWhiteSpace(govId)
                    ? await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.Id != model.Id,
                        disableTracking: true,
                        cancellationToken: cancellationToken)
                    : await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.Id != model.Id,
                        disableTracking: true,
                        cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.NameAr, x => x.NameEn, model.NameAr, model.NameEn))
                    return new ResponseResult().PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                ApartmentType entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                ApartmentType entity = Mapper.Map(model, entityToUpdate);

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        entity.IsDeleted = false;
                }

                SetEntityModifiedBaseProperties(entity);

                UnitOfWork.Repository.Update(entityToUpdate, entity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.UpdateError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.OK, exception: null,
                    message: MessagesConstants.UpdateSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: ex,
                                                 message: MessagesConstants.UpdateError);
            }
        }
    }
}

