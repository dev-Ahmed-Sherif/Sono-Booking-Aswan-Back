using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FuzzySharp;
using LinqKit;
using SonoTracker.Application.Services.Base;
using SonoTracker.Application.Services.LookUp.GeoPoint;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.GeoPoint.Parameters;
using SonoTracker.Common.DTO.Lookup.GeoPoint;
using SonoTracker.Domain;
using SonoTracker.Infrastructure.UnitOfWork;
using SonoTracker.Common.DTO.Lookup.City;
using SonoTracker.Common.DTO.Lookup.AccidentType;

namespace SonoTracker.Application.Services.LookUp.GeoPoint
{
    public class GeoPointService(IServiceBaseParameter<Entities.Lookups.GeoPoint> businessBaseParameter) : BaseService<Entities.Lookups.GeoPoint, AddGeoPointDto, EditGeoPointDto, GeoPointDto, string, string>(businessBaseParameter), IGeoPointService
    {
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Domain.Entities.Lookups.GeoPoint, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            IEnumerable<Domain.Entities.Lookups.GeoPoint> entities = await UnitOfWork.Repository.GetAllAsync(disableTracking: disableTracking, cancellationToken: cancellationToken);

            var filtered = IsSuperAdmin()
                ? (entities ?? Enumerable.Empty<Domain.Entities.Lookups.GeoPoint>())
                : (entities?.Where(e => !e.IsDeleted) ?? Enumerable.Empty<Domain.Entities.Lookups.GeoPoint>());
            IEnumerable<GeoPointDto> mapped = Mapper.Map<IEnumerable<Domain.Entities.Lookups.GeoPoint>, IEnumerable<GeoPointDto>>(filtered);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<GeoPointFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var geoPointFilter = filter?.Filter ?? new GeoPointFilter();

            if (!isSuperAdmin)
                geoPointFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            (int Count, IEnumerable<Entities.Lookups.GeoPoint> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: PredicateBuilderFunction(geoPointFilter),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            var filteredResult = isSuperAdmin
                ? (Result ?? Enumerable.Empty<Entities.Lookups.GeoPoint>())
                : (Result?.Where(x => x.IsDeleted != true) ?? Enumerable.Empty<Entities.Lookups.GeoPoint>());
            var data = Mapper.Map<IEnumerable<Entities.Lookups.GeoPoint>, IEnumerable<GeoPointDto>>(filteredResult);

            return new PagingResult(filter.PageNumber, filter.PageSize, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Lookups.GeoPoint>, IEnumerable<GeoPointDto>>(query.Result.Where(x => x.IsDeleted != true));

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<Entities.Lookups.GeoPoint, bool>> PredicateBuilderFunction(GeoPointFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.GeoPoint>(x => x.IsDeleted == filter.IsDeleted);
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
        static Expression<Func<Entities.Lookups.GeoPoint, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Lookups.GeoPoint>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.NameAr.Contains(filter.SearchCriteria));
                predicate = predicate.Or(b => b.NameEn.Contains(filter.SearchCriteria));
            }
            return predicate;
        }

        private static string NormalizeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var parts = value.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', parts).ToLowerInvariant();
        }

        private static string? FindFuzzyNorthEastDuplicate(IEnumerable<Entities.Lookups.GeoPoint> existing, string north, string east, int threshold = 90)
        {
            var list = existing as IList<Entities.Lookups.GeoPoint> ?? [.. existing];
            var normalizedNorth = NormalizeText(north);
            var normalizedEast = NormalizeText(east);

            var match = list.FirstOrDefault(x =>
                             x.Code == "02" ||
                          ((!string.IsNullOrWhiteSpace(x.North) &&
                            !string.IsNullOrWhiteSpace(normalizedNorth) &&
                             Fuzz.TokenSetRatio(normalizedNorth, NormalizeText(x.North)) >= threshold)
                             ||
                           (!string.IsNullOrWhiteSpace(x.East) &&
                            !string.IsNullOrWhiteSpace(normalizedEast) &&
                             Fuzz.TokenSetRatio(normalizedEast, NormalizeText(x.East)) >= threshold)));

            return match?.Id;
        }

        public override async Task<IFinalResult> AddAsync(AddGeoPointDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingForDup = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.IsDeleted != true && x.NameAr == model.NameAr,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                var duplicateId = FindFuzzyNorthEastDuplicate(existingForDup, model.North, model.East);
                if (duplicateId is not null)
                    return ResponseResult.PostResult(result: duplicateId, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                var entity = Mapper.Map<Entities.Lookups.GeoPoint>(model);
                entity.GovernorateId = GetGovernorateIdFromClaims();

                SetEntityCreatedBaseProperties(entity);
                var result = await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
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
        public override async Task<IFinalResult> UpdateAsync(AddGeoPointDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingForDup = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.Id != model.Id && x.IsDeleted != true && x.NameAr == model.NameAr,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                var duplicateId = FindFuzzyNorthEastDuplicate(existingForDup, model.North, model.East);
                if (duplicateId is not null)
                    return ResponseResult.PostResult(result: duplicateId, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                Entities.Lookups.GeoPoint entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

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
