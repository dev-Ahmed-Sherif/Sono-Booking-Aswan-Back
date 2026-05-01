using Azure.Core;
using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SonoTracker.Application.Services.Base;
using SonoTracker.Application.Services.Lookup.City;
using SonoTracker.Application.Services.Lookup.Town;
using SonoTracker.Application.Services.LookUp.GeoPoint;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.City;
using SonoTracker.Common.DTO.Lookup.GeoPoint;
using SonoTracker.Common.DTO.Lookup.Town;
using SonoTracker.Common.DTO.Tracker.TouristMarina;
using SonoTracker.Common.DTO.Tracker.TouristMarina.Parameters;
using SonoTracker.Common.Helpers;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Domain;
using SonoTracker.Domain.Entities.Tracker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.Marinas
{
    public class MarinaOrganizationService : BaseService<TouristMarina, AddTouristMarinaDto, EditTouristMarinaDto, TouristMarinaDto, string, string>, ITouristMarinaService
    {
        private readonly ICityService _cityService;
        private readonly IGeoPointService _geoPointService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;

        public MarinaOrganizationService(
            IServiceBaseParameter<TouristMarina> businessBaseParameter,
            ICityService cityService, IGeoPointService geoPointService,
            IWebHostEnvironment hostingEnvironment, IHttpContextAccessor request) : base(businessBaseParameter)
        {
            _cityService = cityService;
            _geoPointService = geoPointService;
            _hostingEnvironment = hostingEnvironment;
            _request = request;
            _uploaderConfiguration = new UploaderConfiguration(_hostingEnvironment, _request);
        }

        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            TouristMarina entity = await UnitOfWork.Repository
                                            .FirstOrDefaultAsync(x => x.Id.Equals(id.ToString()), include: src => src
                                            .Include(t => t.City)
                                            .Include(x => x.GeoPoint),
                                             cancellationToken: cancellationToken);

            EditTouristMarinaDto mapped = Mapper.Map<TouristMarina, EditTouristMarinaDto>(entity);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            TouristMarina entity = await UnitOfWork.Repository
                                    .FirstOrDefaultAsync(x => x.Id.Equals(id.ToString()), include: src => src
                                    .Include(t => t.City)
                                    .Include(x => x.GeoPoint),
                                    cancellationToken: cancellationToken);

            TouristMarinaDto mapped = Mapper.Map<TouristMarina, TouristMarinaDto>(entity);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
        }

        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<TouristMarina, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            IEnumerable<TouristMarina> entity = await UnitOfWork.Repository
                                         .GetAllAsync(include: src => src
                                         .Include(t => t.City)
                                         .Include(x => x.GeoPoint),
                                          cancellationToken: cancellationToken);

            string governorateId = IsSuperAdmin() ? null : GetGovernorateIdFromClaims();

            IEnumerable<TouristMarina> filteredEntities = IsSuperAdmin()
                ? entity
                : entity.Where(e => !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.GovernorateId == governorateId));

            IEnumerable<TouristMarinaDto> mapped = Mapper.Map<IEnumerable<TouristMarina>, IEnumerable<TouristMarinaDto>>(filteredEntities);

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
        }
        public async Task<IFinalResult> GetAllFilterAsync(TouristMarinaFilter filter, CancellationToken cancellationToken = default)
        {
            IEnumerable<TouristMarina> entity = await UnitOfWork.Repository
                                .FindAsync(predicate: PredicateBuilderFunction(filter), cancellationToken: cancellationToken);

            IEnumerable<TouristMarinaDto> data = Mapper.Map<IEnumerable<TouristMarina>, IEnumerable<TouristMarinaDto>>(entity.Where(x => x.IsDeleted != true));

            return ResponseResult.PostResult(result: data, status: HttpStatusCode.OK, exception: null,
                                             message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<TouristMarinaFilter> filter, CancellationToken cancellationToken = default)
        {
            bool isSuperAdmin = IsSuperAdmin();

            TouristMarinaFilter touristMarinaFilter = filter?.Filter ?? new TouristMarinaFilter();

            string governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            if (!isSuperAdmin)
                touristMarinaFilter.IsDeleted = false;

            int limit = filter.PageSize;

            int offset = --filter.PageNumber * filter.PageSize;

            (int Count, IEnumerable<TouristMarina> Result) query = await UnitOfWork.Repository
                .FindPagedAsync(
                    predicate: PredicateBuilderFunction(touristMarinaFilter, governorateId),
                    pageNumber: offset,
                    pageSize: limit, filter.OrderByValue,
                    include: src => src
                             .Include(t => t.GeoPoint)
                             .Include(x => x.City),
                    cancellationToken: cancellationToken);

            IEnumerable<TouristMarina> items = isSuperAdmin ? query.Result : query.Result.Where(x => x.IsDeleted != true);

            IEnumerable<TouristMarinaDto> data = Mapper.Map<IEnumerable<TouristMarina>, IEnumerable<TouristMarinaDto>>(items);

            return new PagingResult(pageNumber: filter.PageNumber, pageSize: filter.PageSize, totalCount: query.Count, result: data,
                                    status: HttpStatusCode.OK, message: MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            int limit = filter.PageSize;

            int offset = --filter.PageNumber * filter.PageSize;

            bool isSuperAdmin = IsSuperAdmin();

            Expression<Func<TouristMarina, bool>> predicate = DropDownPredicateBuilderFunction(filter.Filter);

            (int Count, IEnumerable<TouristMarina> Result) query = await UnitOfWork.Repository.FindPagedAsync(
                    predicate: predicate,
                    pageNumber: offset,
                    pageSize: limit,
                    cancellationToken: cancellationToken);

            IEnumerable<TouristMarina> items = isSuperAdmin ? query.Result : query.Result.Where(x => x.IsDeleted != true);

            IEnumerable<TouristMarinaDto> data = Mapper.Map<IEnumerable<TouristMarina>, IEnumerable<TouristMarinaDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        static Expression<Func<TouristMarina, bool>> PredicateBuilderFunction(TouristMarinaFilter filter, string governorateId = null)
        {
            var predicate = PredicateBuilder.New<TouristMarina>(x => x.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.CityId))
            {
                predicate = predicate.And(x => x.CityId == filter.CityId);
            }
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                predicate = predicate.And(x => x.NameAr.Contains(filter.Name));
            }
            if (!string.IsNullOrWhiteSpace(filter.Code))
            {
                predicate = predicate.And(x => x.Code.Contains(filter.Code));
            }

            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x => x.GovernorateId == governorateId);
            }

            return predicate;
        }
        static Expression<Func<TouristMarina, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<TouristMarina>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.Code.Contains(filter.SearchCriteria));
                //  predicate = predicate.Or(b => b.Name.Contains(filter.SearchCriteria));
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


        public override async Task<IFinalResult> AddAsync(AddTouristMarinaDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                await AssignMarinaCodeFromCityAsync(model, cancellationToken);

                bool isSuperAdmin = IsSuperAdmin();

                string govId = GetGovernorateIdFromClaims();

                IEnumerable<TouristMarina> existingForDup = isSuperAdmin || string.IsNullOrWhiteSpace(govId)
                    ? await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.IsDeleted != true,
                        disableTracking: true,
                        cancellationToken: cancellationToken)
                    : await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.GovernorateId == govId && x.IsDeleted != true,
                        disableTracking: true,
                        cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, m => m.NameAr, m => string.Empty, model.NameAr, string.Empty))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                TouristMarina entity = Mapper.Map<TouristMarina>(model);
                entity.GovernorateId = govId;
                SetEntityCreatedBaseProperties(entity);

                if (model.ImageUrl != null)
                {
                    string res = await _uploaderConfiguration.UploadFile(model.ImageUrl, $"Marina", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    entity.ImageUrl = res;
                }

                if (model.NorthGeo != null && model.EastGeo != null)
                {
                    IFinalResult geoPoint = await _geoPointService.AddAsync(
                    new AddGeoPointDto
                    {
                        Code = "01",
                        NameAr = $"مرسى + {model.NameAr}",
                        NameEn = $"Marina + {model.NameEn}",
                        North = model.NorthGeo,
                        East = model.EastGeo
                    }, cancellationToken);

                    if (geoPoint.Data is false)
                        return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                         message: MessagesConstants.AddError);

                    entity.GeoPointId = geoPoint.Data?.ToString();
                }

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
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                                                 message: MessagesConstants.AddError + ex.Message);
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddTouristMarinaDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                bool isSuperAdmin = IsSuperAdmin();

                string govId = GetGovernorateIdFromClaims();

                IEnumerable<TouristMarina> existingForDup = isSuperAdmin || string.IsNullOrWhiteSpace(govId)
                    ? await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.Id != dto.Id && x.IsDeleted != true,
                        disableTracking: true,
                        cancellationToken: cancellationToken)
                    : await UnitOfWork.Repository.FindAsync(
                        predicate: x => x.GovernorateId == govId && x.Id != dto.Id && x.IsDeleted != true,
                        disableTracking: true,
                        cancellationToken: cancellationToken);

                if (LookupDuplicateGuard.HasFuzzyNameDuplicate(existingForDup, m => m.NameAr, m => string.Empty, dto.NameAr, string.Empty))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                TouristMarina entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, dto.Id);

                string currentImageUrl = entityToUpdate.ImageUrl;

                TouristMarina newEntity = Mapper.Map(dto, entityToUpdate);
                newEntity.GovernorateId = GetGovernorateIdFromClaims();
                SetEntityModifiedBaseProperties(newEntity);

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        newEntity.IsDeleted = false;
                }

                if (dto.ImageUrl != null)
                {
                    string res = await _uploaderConfiguration.UploadFile(dto.ImageUrl, $"Marina", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    _uploaderConfiguration.DeleteFile(currentImageUrl);

                    newEntity.ImageUrl = res;
                }
                else
                {
                    newEntity.ImageUrl = currentImageUrl;
                }

                IFinalResult geoPoint = await _geoPointService.UpdateAsync(
                new AddGeoPointDto
                {
                    Id = dto.GeoPointId,
                    Code = "01",
                    NameAr = $"مرسى + {dto.NameAr}",
                    NameEn = $"Marina + {dto.NameEn}",
                    North = dto.NorthGeo,
                    East = dto.EastGeo
                }, cancellationToken);

                UnitOfWork.Repository.Update(entityToUpdate, newEntity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

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

        private async Task AssignMarinaCodeFromCityAsync(AddTouristMarinaDto model, CancellationToken cancellationToken)
        {
            IFinalResult city = await _cityService.GetByIdAsync(model.CityId, cancellationToken);

            if (city.Data is not CityDto cityDto)
                return;

            string cityCode = cityDto.Code ?? string.Empty;

            TouristMarinaFilter filter = new()
            {
                CityId = model.CityId,
                IsDeleted = false
            };

            IFinalResult exist = await GetAllFilterAsync(filter, cancellationToken);

            ICollection<TouristMarinaDto> existDataCollection = exist.Data as ICollection<TouristMarinaDto>;

            int nextSeq = 1;

            if (existDataCollection is { Count: > 0 })
            {
                TouristMarinaDto lastItem = existDataCollection
                                .Where(x => x.Code != null &&
                                    x.Code.Length >= cityCode.Length + 3 &&
                                    x.Code.StartsWith(cityCode, StringComparison.Ordinal))
                                .OrderByDescending(x => x.Code)
                                .FirstOrDefault();

                if (lastItem?.Code != null &&
                    int.TryParse(lastItem.Code.AsSpan(lastItem.Code.Length - 3), out var number))
                    nextSeq = number + 1;
            }

            model.Code = cityCode + nextSeq.ToString("D3");
        }

        private IFinalResult UploadResponse(string res)
        {
            if (res == "Size")
            {
                var message = "File Size Larger than 5 Mega Bytes";
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, message: message);
            }
            else if (res == "Type")
            {
                var message = "File type not allowed.";
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, message: message);
            }
            else
            {
                return null;
            }
        }
    }
}
