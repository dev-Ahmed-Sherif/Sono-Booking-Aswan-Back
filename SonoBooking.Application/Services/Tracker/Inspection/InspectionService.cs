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
using SonoTracker.Common.DTO.Tracker.GeneralInspection;
using SonoTracker.Common.DTO.Tracker.GeneralInspection.Parameters;
using SonoTracker.Common.DTO.Tracker.Inspection;
using SonoTracker.Common.DTO.Tracker.Inspection.Parameters;
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

namespace SonoTracker.Application.Services.Tracker.Inspection
{
    public class InspectionService : BaseService<Entities.Tracker.Inspection, AddInspectionDto, EditInspectionDto, InspectionDto, string, string>, IInspectionService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;
        private readonly IFloatingUnitService _floatingUnitService;
        private readonly IAttachService _attachService;
        public InspectionService(IServiceBaseParameter<Entities.Tracker.Inspection> businessBaseParameter,
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
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr, include: src => src
                .Include(t => t.Organization)
                .Include(t => t.FloatingUnit)
                , cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.Inspection, EditInspectionDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr,
              include: src => src
                .Include(t => t.Organization)
                .Include(t => t.FloatingUnit), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.Inspection, InspectionDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.Inspection, bool>> predicate = null, CancellationToken cancellationToken = default)
            => await GetAllAsync(inspectionTypeId: null, cancellationToken);

        public async Task<IFinalResult> GetAllAsync(string inspectionTypeId, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            var entity = await UnitOfWork.Repository.GetAllAsync(
                include: src => src
                    .Include(t => t.Organization)
                    .Include(t => t.FloatingUnit),
                cancellationToken: cancellationToken);

            var filteredEntities = isSuperAdmin
                ? entity
                : entity.Where(e => !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.GovernorateId == governorateId));

            if (!string.IsNullOrEmpty(inspectionTypeId))
                filteredEntities = filteredEntities.Where(e => e.InspectionTypeId == inspectionTypeId);

            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.Inspection>, IEnumerable<InspectionDto>>(filteredEntities);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<InspectionFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var inspectionFilter = filter?.Filter ?? new InspectionFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();
            if (!isSuperAdmin)
                inspectionFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(inspectionFilter, governorateId), pageNumber: offset, pageSize: limit, filter.OrderByValue,
                include: src => src
                .Include(t => t.Organization)
                .Include(t => t.FloatingUnit),
                cancellationToken: cancellationToken);

            var items = isSuperAdmin ? query.Item2 : query.Item2.Where(x => x.IsDeleted != true);
            var data = Mapper.Map<IEnumerable<Entities.Tracker.Inspection>, IEnumerable<InspectionDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<IFinalResult> GetAllFilterAsync(InspectionFilter filter, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FindAsync(predicate: PredicateBuilderFunction(filter), cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.Inspection>, IEnumerable<InspectionDto>>(entity.Where(x => x.IsDeleted != true));

            return ResponseResult.PostResult(data, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        static Expression<Func<Entities.Tracker.Inspection, bool>> PredicateBuilderFunction(InspectionFilter filter, string governorateId = null)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.Inspection>(x => x.IsDeleted == filter.IsDeleted);
           
            if (!string.IsNullOrEmpty(filter.OrganizationId))
            {
                predicate = predicate.And(x => x.OrganizationId == filter.OrganizationId);
            }
            if (filter.InspectionDate.HasValue)
            {
                predicate = predicate.And(x => x.InspectionDate == DateOnly.FromDateTime(filter.InspectionDate.Value));
            }
            if (!string.IsNullOrEmpty(filter.FloatingUnitId))
            {
                predicate = predicate.And(x => x.FloatingUnitId == filter.FloatingUnitId);
            }
            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x => x.GovernorateId == governorateId);
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

        public override async Task<IFinalResult> AddAsync(AddInspectionDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var mapped = Mapper.Map<Entities.Tracker.Inspection>(dto);

                SetEntityCreatedBaseProperties(mapped);

                var floatingUnit = await _floatingUnitService.GetByIdAsync(dto.FloatingUnitId, cancellationToken);

                string floatingUnitCode;

                if (floatingUnit.Data is FloatingUnitDto floatingUnitDto)
                {
                    floatingUnitCode = floatingUnitDto.Code;

                    InspectionFilter filter = new()
                    {
                        FloatingUnitId = dto.FloatingUnitId,
                        IsDeleted = false
                    };

                    // Check if the trip information already exists for the given floating unit id
                    var exist = await GetAllFilterAsync(filter, cancellationToken);
                    // Fix: Explicitly cast exist.Data to a collection type to access the Count property.
                    var existDataCollection = exist.Data as ICollection<InspectionDto>;

                    if (existDataCollection?.Count > 0)
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

                if (dto.InspectionAttachment != null)
                {
                    foreach (var formFile in dto.InspectionAttachment)
                    {
                        Guid guid = Guid.NewGuid();
                        var AddDto = new AddAttachDto
                        {
                            Id = guid.ToString(),
                            Path = formFile,
                            AttachType = "Inspection",
                        };

                        IFinalResult Attach = await _attachService.AddAsync(AddDto, cancellationToken);

                        mapped.InspectionAttachments.Add(new Entities.Tracker.InspectionAttachment
                        {
                            AttachmentId = (string)Attach.Data,
                            InspectionId = mapped.Id
                        });
                    }
                }

                mapped.GovernorateId = GetGovernorateIdFromClaims();

                mapped.IsDeleted = false;

                if (dto.InspectionFloatingUnitClauses?.Count > 0)
                {
                    foreach (var clauseDto in dto.InspectionFloatingUnitClauses)
                    {
                        mapped.InspectionFloatingUnitClauses.Add(new Entities.Tracker.InspectionFloatingUnitClause
                        {
                            IsInspected = clauseDto.IsInspected,
                            Number = clauseDto.Number,
                            Note = clauseDto.Note,
                            InspectionId = mapped.Id,
                            InspectionClauseId = clauseDto.InspectionClauseId
                        });
                    }
                }

                UnitOfWork.Repository.Add(mapped);
                var rows = await UnitOfWork.SaveChangesAsync(cancellationToken);
                if (rows == 0)
                    return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.AddError);

                return ResponseResult.PostResult(mapped, status: HttpStatusCode.Created, exception: null,
                    message: HttpStatusCode.Created.ToString());
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.AddError + ex.Message);
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddInspectionDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                // disableTracking: false — EF Core must track the entity and its child collection
                // so that Clear() marks old rows as Deleted and Add() marks new rows as Added.
                var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id == dto.Id,
                    include: src => src.Include(i => i.InspectionFloatingUnitClauses),
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                // Map scalar properties only (InspectionFloatingUnitClauses is ignored in the profile)
                Mapper.Map(dto, entity);
                entity.GovernorateId = GetGovernorateIdFromClaims();
                SetEntityModifiedBaseProperties(entity);

                if (IsSuperAdmin() && entity.IsDeleted)
                    entity.IsDeleted = false;

                if (dto.InspectionAttachment != null)
                {
                    foreach (var formFile in dto.InspectionAttachment)
                    {
                        Guid guid = Guid.NewGuid();
                        var AddDto = new AddAttachDto
                        {
                            Id = guid.ToString(),
                            Path = formFile,
                            AttachType = "Inspection",
                        };

                        IFinalResult Attach = await _attachService.AddAsync(AddDto, cancellationToken);

                        entity.InspectionAttachments.Add(new Entities.Tracker.InspectionAttachment
                        {
                            AttachmentId = (string)Attach.Data,
                            InspectionId = entity.Id
                        });
                    }
                }


                // Explicitly mark old clause rows as Deleted BEFORE clearing the collection.
                // Without this, EF Core's orphan cleanup generates UPDATE SET InspectionId = NULL
                // which fails because InspectionId is NOT NULL in the database.
                var clauseRepo = UnitOfWork.GetRepository<Entities.Tracker.InspectionFloatingUnitClause>();
                var oldClauses = entity.InspectionFloatingUnitClauses.ToList();
                if (oldClauses.Count > 0)
                    clauseRepo.RemoveRange(oldClauses);

                entity.InspectionFloatingUnitClauses.Clear();

                if (dto.InspectionFloatingUnitClauses?.Count > 0)
                {
                    foreach (var clauseDto in dto.InspectionFloatingUnitClauses)
                    {
                        entity.InspectionFloatingUnitClauses.Add(new Entities.Tracker.InspectionFloatingUnitClause
                        {
                            IsInspected = clauseDto.IsInspected,
                            Number = clauseDto.Number,
                            Note = clauseDto.Note,
                            InspectionId = entity.Id,
                            InspectionClauseId = clauseDto.InspectionClauseId
                        });
                    }
                }

                // No explicit Update() call needed — the change tracker already knows
                // about all scalar and collection changes from the tracked entity.
                var affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);
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
        public override async Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {

            try
            {
                var entityToDelete = await UnitOfWork.Repository.GetAsync(id);

                // Reomve Uploaded File
                _uploaderConfiguration.DeleteFile(entityToDelete.InspectionAttachment);

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
