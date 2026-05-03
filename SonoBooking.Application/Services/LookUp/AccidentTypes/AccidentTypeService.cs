using LinqKit;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Helpers;
using SonoTracker.Common.Core;
using SonoTracker.Common.Constants.Auth;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.AccidentType;
using SonoTracker.Common.DTO.Lookup.AccidentType.Parameters;
using SonoTracker.Common.DTO.Lookup.City;
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

namespace SonoTracker.Application.Services.LookUp.AccidentTypes
{
    public class AccidentTypeService(IServiceBaseParameter<BookingType> businessBaseParameter) : BaseService<BookingType, AddAccidentTypeDto, EditAccidentTypeDto, AccidentTypeDto, string, string>(businessBaseParameter), IAccidentTypeService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<BookingType, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            string governorateId = IsSuperAdmin() ? null : GetGovernorateIdFromClaims();

            IEnumerable<BookingType> entities = predicate != null
                ? await UnitOfWork.Repository.FindAsync(predicate, cancellationToken: cancellationToken)
                : await UnitOfWork.Repository.GetAllAsync(disableTracking: disableTracking, cancellationToken: cancellationToken);

            IEnumerable<BookingType> filtered = IsSuperAdmin()
                ? (entities ?? [])
                : (entities?.Where(e => !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.GovernorateId == governorateId)) ?? []);

            IEnumerable<AccidentTypeDto> mapped = Mapper.Map<IEnumerable<BookingType>, IEnumerable<AccidentTypeDto>>(filtered);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<AccidentTypeFilter> filter, CancellationToken cancellationToken = default)
        {
            bool isSuperAdmin = IsSuperAdmin();

            string governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            int limit = filter.PageSize;

            int offset = --filter.PageNumber * filter.PageSize;

            AccidentTypeFilter accidentFilter = filter?.Filter ?? new AccidentTypeFilter();

            if (!isSuperAdmin)
                accidentFilter.IsDeleted = false;

            (int Count, IEnumerable<BookingType> Result) =
                 await UnitOfWork.Repository.FindPagedAsync(
                 predicate: PredicateBuilderFunction(accidentFilter, governorateId),
                 pageNumber: offset,
                 pageSize: limit,
                 filter.OrderByValue,
                 cancellationToken: cancellationToken);

            IEnumerable<BookingType> filteredResult = isSuperAdmin ? (Result ?? []) : (Result?.Where(x => !x.IsDeleted) ?? []);

            IEnumerable<AccidentTypeDto> data = Mapper.Map<IEnumerable<BookingType>, IEnumerable<AccidentTypeDto>>(filteredResult);

            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            int limit = filter.PageSize;

            int offset = --filter.PageNumber * filter.PageSize;

            Expression<Func<BookingType, bool>> predicate = DropDownPredicateBuilderFunction(filter.Filter);

            (int Count, IEnumerable<BookingType> Result) query =
                await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            IEnumerable<AccidentTypeDto> data = Mapper.Map<IEnumerable<BookingType>, IEnumerable<AccidentTypeDto>>(query.Result.Where(x => x.IsDeleted != true));

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        static Expression<Func<BookingType, bool>> PredicateBuilderFunction(AccidentTypeFilter filter, string governorateId)
        {
            var predicate = PredicateBuilder.New<BookingType>(x => x.IsDeleted == filter.IsDeleted);

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
        static Expression<Func<BookingType, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<BookingType>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.NameEn.Contains(filter.SearchCriteria));
            }
            return predicate;
        }
        public override async Task<IFinalResult> AddAsync(AddAccidentTypeDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                bool isSuperAdmin = IsSuperAdmin();

                string govId = GetGovernorateIdFromClaims();

                IEnumerable<BookingType> existingForDup = isSuperAdmin || string.IsNullOrWhiteSpace(govId)
                    ? await UnitOfWork.Repository.FindAsync(disableTracking: true, cancellationToken: cancellationToken)
                    : await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.GovernorateId == govId,
                        disableTracking: true,
                        cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, x => x.NameAr, x => x.NameEn, model.NameAr, model.NameEn))
                    return new ResponseResult().PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                BookingType entity = Mapper.Map<BookingType>(model);

                entity.GovernorateId = GetGovernorateIdFromClaims();

                SetEntityCreatedBaseProperties(entity);

                IFinalResult lastEntity = await GetLastRecordAsync(cancellationToken);

                if (lastEntity.Data != null)
                {
                    if (lastEntity.Data is AccidentTypeDto accidentTypeDto)
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

                BookingType result = await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

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
        public override async Task<IFinalResult> UpdateAsync(AddAccidentTypeDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                bool isSuperAdmin = IsSuperAdmin();

                string govId = GetGovernorateIdFromClaims();

                IEnumerable<BookingType> existingForDup = isSuperAdmin || string.IsNullOrWhiteSpace(govId)
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

                BookingType entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                BookingType entity = Mapper.Map(model, entityToUpdate);
                entity.GovernorateId = GetGovernorateIdFromClaims();

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
