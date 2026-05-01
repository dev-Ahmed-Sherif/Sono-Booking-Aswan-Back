using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SonoTracker.Application.Services.Base;
using SonoTracker.Application.Services.LookUp.Attach;
using SonoTracker.Application.Services.Tracker.FloatingUnits;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Lookup.Attach;
using SonoTracker.Common.DTO.Tracker.FloatingUnit;
using SonoTracker.Common.DTO.Tracker.Maintenance;
using SonoTracker.Common.DTO.Tracker.Maintenance.Parameters;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.Maintenance
{

    public class MaintenanceService : BaseService<Domain.Entities.Tracker.Maintenance, AddMaintenanceDto, EditMaintenanceDto, MaintenanceDto, string, string>, IMaintenanceService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;
        private readonly IFloatingUnitService _floatingUnitService;
        private readonly IAttachService _attachService;

        public MaintenanceService(IServiceBaseParameter<Entities.Tracker.Maintenance> businessBaseParameter,
            IWebHostEnvironment hostingEnvironment, IHttpContextAccessor request,
            IFloatingUnitService floatingUnitService, IAttachService attachService) : base(businessBaseParameter)
        {
            _hostingEnvironment = hostingEnvironment;
            _request = request;
            _uploaderConfiguration = new UploaderConfiguration(_hostingEnvironment, _request);
            _floatingUnitService = floatingUnitService;
            _attachService = attachService;
        }
        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src
                .Include(t => t.FloatingUnit)
               .Include(x => x.MaintenanceType), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Domain.Entities.Tracker.Maintenance, EditMaintenanceDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
                include: src => src
                .Include(t => t.FloatingUnit)
               .Include(x => x.MaintenanceType), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Domain.Entities.Tracker.Maintenance, MaintenanceDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Domain.Entities.Tracker.Maintenance, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.GetAllAsync(include: src => src
             .Include(t => t.FloatingUnit)
               .Include(x => x.MaintenanceType), cancellationToken: cancellationToken);
            var governorateId = IsSuperAdmin() ? null : GetGovernorateIdFromClaims();
            var filteredEntities = IsSuperAdmin()
                ? entity
                : entity.Where(e => !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.GovernorateId == governorateId));
            var mapped = Mapper.Map<IEnumerable<Domain.Entities.Tracker.Maintenance>, IEnumerable<MaintenanceDto>>(filteredEntities);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<MaintenanceFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var maintenanceFilter = filter?.Filter ?? new MaintenanceFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            if (!isSuperAdmin)
                maintenanceFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(maintenanceFilter, governorateId),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    include: src => src
                     .Include(t => t.FloatingUnit)
                     .Include(x => x.MaintenanceType),
                    cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.Maintenance>, IEnumerable<MaintenanceDto>>(query.Item2);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var items = isSuperAdmin ? query.Item2 : query.Item2.Where(x => x.IsDeleted != true);
            var data = Mapper.Map<IEnumerable<Entities.Tracker.Maintenance>, IEnumerable<MaintenanceDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);

        }


        static Expression<Func<Entities.Tracker.Maintenance, bool>> PredicateBuilderFunction(MaintenanceFilter filter, string governorateId = null)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.Maintenance>(x => x.IsDeleted == filter.IsDeleted);

            //if (filter.TownId.HasValue)
            //{
            //    predicate = predicate.And(x => x.TownId == filter.TownId.Value);
            //}
            //if (!string.IsNullOrWhiteSpace(filter.Name))
            //{
            //    predicate = predicate.And(x => x.Name.Contains(filter.Name));
            //}
            //if (!string.IsNullOrWhiteSpace(filter.Code))
            //{
            //    predicate = predicate.And(x => x.Code.Contains(filter.Code));
            //}

            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x => x.GovernorateId == governorateId);
            }

            return predicate;
        }
        static Expression<Func<Entities.Tracker.Maintenance, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.Maintenance>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {

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

        public override async Task<IFinalResult> AddAsync(AddMaintenanceDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var mapped = Mapper.Map<Entities.Tracker.Maintenance>(dto);

                mapped.GovernorateId = GetGovernorateIdFromClaims();

                var floatingUnit = await _floatingUnitService.GetByIdAsync(dto.FloatingUnitId, cancellationToken);

                string floatingUnitCode;

                if (floatingUnit.Data is FloatingUnitDto floatingUnitDto)
                {
                    floatingUnitCode = floatingUnitDto.Code;

                    MaintenanceFilter filter = new()
                    {
                        FloatingUnitId = dto.FloatingUnitId,
                        IsDeleted = false
                    };

                    // Check if the trip information already exists for the given floating unit id
                    var exist = await GetAllFilterAsync(filter, cancellationToken);
                    // Fix: Explicitly cast exist.Data to a collection type to access the Count property.
                    var existDataCollection = exist.Data as ICollection<MaintenanceDto>;

                    if (existDataCollection?.Count > 0 &&
                        DateTime.UtcNow.Month != 1 &&
                        DateTime.UtcNow.Day != 1)
                    {
                        // Handle existing trip information logic here (if needed)
                        var lastTrip = existDataCollection.LastOrDefault();
                        
                        if (lastTrip != null && lastTrip.Number.StartsWith(floatingUnitCode))
                        {
                            // Extract the numeric part and increment it
                            var numericPart = lastTrip.Number[floatingUnitCode.Length..];
                            if (int.TryParse(numericPart, out int number))
                            {
                                number++;
                                mapped.Number = floatingUnitCode + number.ToString("D2"); // Ensure 4 digits
                            }
                        }
                    }
                    else
                    {
                        mapped.Number = floatingUnitCode + "01";
                    }
                }

                if (dto.MaintenanceReport != null)
                {
                    string res = await _uploaderConfiguration.UploadFile(dto.MaintenanceReport, "Maintenance/Report", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    mapped.MaintenanceReport = res;
                }

                if (dto.Other != null)
                {
                    foreach (var formFile in dto.Other)
                    {
                        Guid guid = Guid.NewGuid();
                        var AddDto = new AddAttachDto
                        {
                            Id = guid.ToString(),
                            Path = formFile,
                            AttachType = "Maintenance/Other",
                        };

                        IFinalResult Attach = await _attachService.AddAsync(AddDto, cancellationToken);

                        mapped.MaintenanceAttachments.Add(new Entities.Tracker.MaintenanceAttachment
                        {
                            AttachmentId = (string)Attach.Data,
                            MaintenanceId = mapped.Id
                        });
                    }
                }
                
                mapped.IsDeleted = false;

                SetEntityCreatedBaseProperties(mapped);

                UnitOfWork.Repository.Add(mapped);

                int rows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                return ResponseResult.PostResult(mapped, status: HttpStatusCode.Created, message: HttpStatusCode.Created.ToString());
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                                                 message: MessagesConstants.AddError + ex.Message);
            }
        }


        public override async Task<IFinalResult> UpdateAsync(AddMaintenanceDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                Entities.Tracker.Maintenance entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, dto.Id);

                string currentMaintenanceReport = entityToUpdate.MaintenanceReport;

                Entities.Tracker.Maintenance newEntity = Mapper.Map(dto, entityToUpdate);
                
                newEntity.GovernorateId = GetGovernorateIdFromClaims();

                SetEntityModifiedBaseProperties(newEntity);

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        newEntity.IsDeleted = false;
                }

                if (entityToUpdate != null)
                {
                    if (dto.MaintenanceReport != null)
                    {
                        string res = await _uploaderConfiguration.UploadFile(dto.MaintenanceReport, "Maintenance/Report", cancellationToken);

                        if (res != null)
                        {
                            if (UploadResponse(res) != null)
                                return UploadResponse(res);
                        }

                        _uploaderConfiguration.DeleteFile(entityToUpdate.MaintenanceReport);

                        newEntity.MaintenanceReport = res;
                    }
                    else
                    {
                        newEntity.MaintenanceReport = currentMaintenanceReport;
                    }

                    if (dto.Other != null)
                    {
                        foreach (var formFile in dto.Other)
                        {
                            Guid guid = Guid.NewGuid();
                            var AddDto = new AddAttachDto
                            {
                                Id = guid.ToString(),
                                Path = formFile,
                                AttachType = "Maintenance/Other",
                            };

                            IFinalResult Attach = await _attachService.AddAsync(AddDto, cancellationToken);

                            newEntity.MaintenanceAttachments.Add(new Entities.Tracker.MaintenanceAttachment
                            {
                                AttachmentId = (string)Attach.Data,
                                MaintenanceId = entityToUpdate.Id
                            });
                        }
                    }
                }

                UnitOfWork.Repository.Update(entityToUpdate, newEntity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.UpdateError);

                return ResponseResult.PostResult(result: newEntity.Id, status: HttpStatusCode.Created, exception: null,
                                                 message: MessagesConstants.UpdateSuccess);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"{MessagesConstants.UpdateError}-{nameof(UpdateAsync)}");
                //_logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: ex,
                                                 message: MessagesConstants.UpdateError);
            }

        }
        public override async Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {

            try
            {
                var entityToDelete = await UnitOfWork.Repository.GetAsync(cancellationToken, id);

                // Reomve Uploaded File
                _uploaderConfiguration.DeleteFile(entityToDelete.MaintenanceReport);
                //_uploaderConfiguration.DeleteFile(entityToDelete.OtherAttach);

                UnitOfWork.Repository.Remove(entityToDelete);
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);
                if (affectedRows > 0)
                {
                    Result = ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted,
                        message: MessagesConstants.DeleteSuccess);
                }

                return Result;
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.DeleteError + ex.Message);
            }

        }
        public async Task<IFinalResult> GetAllFilterAsync(MaintenanceFilter filter, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FindAsync(predicate: PredicateBuilderFunction(filter), cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.Maintenance>, IEnumerable<MaintenanceDto>>(entity.Where(x => x.IsDeleted != true));

            return ResponseResult.PostResult(data, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
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
