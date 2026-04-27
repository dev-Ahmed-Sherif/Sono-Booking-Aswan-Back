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
using SonoTracker.Common.DTO.Tracker.Accident;
using SonoTracker.Common.DTO.Tracker.Accident.Parameters;
using SonoTracker.Common.DTO.Tracker.FloatingUnit;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.Accident
{
    public class AccidentService : BaseService<Entities.Tracker.Accident, AddAccidentDto, EditAccidentDto, AccidentDto, string, string>, IAccidentService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;
        private readonly IFloatingUnitService _floatingUnitService;
        private readonly IAttachService _attachService;

        public AccidentService(IServiceBaseParameter<Entities.Tracker.Accident> businessBaseParameter,
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
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(),
                include: src => src
                .Include(t => t.FloatingUnit)
               .Include(x => x.AccidentType)
               .Include(x => x.AccidentOrganizations).ThenInclude(ao => ao.Organization), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Domain.Entities.Tracker.Accident, EditAccidentDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }
        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == id.ToString(),
                               include: src => src
                               .Include(t => t.FloatingUnit)
                               .Include(x => x.AccidentType)
                               .Include(a => a.City) 
                               .Include(x => x.AccidentOrganizations)
                               .ThenInclude(ao => ao.Organization),
                               cancellationToken: cancellationToken);

            var mapped = Mapper.Map<Domain.Entities.Tracker.Accident, AccidentDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }
        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Domain.Entities.Tracker.Accident, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.GetAllAsync(include: src => src
                                         .Include(t => t.FloatingUnit)
                                         .Include(x => x.AccidentType)
                                         .Include(x => x.AccidentOrganizations).ThenInclude(ao => ao.Organization)
                                         .Include(a => a.City), cancellationToken: cancellationToken);

            var governorateId = IsSuperAdmin() ? null : GetGovernorateIdFromClaims();
            var filteredEntities = IsSuperAdmin()
                ? entity
                : entity.Where(e => !e.IsDeleted && (string.IsNullOrWhiteSpace(governorateId) || e.GovernorateId == governorateId));

            var mapped = Mapper.Map<IEnumerable<Domain.Entities.Tracker.Accident>, IEnumerable<AccidentDto>>(filteredEntities);

            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<AccidentFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var accidentFilter = filter?.Filter ?? new AccidentFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            // Non-superadmin users cannot request deleted rows
            if (!isSuperAdmin)
                accidentFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(accidentFilter, governorateId),
                    pageNumber: offset,
                    pageSize: limit,
                    filter.OrderByValue,
                    include: src => src
                      .Include(t => t.FloatingUnit)
                      .Include(x => x.AccidentType)
                      .Include(x => x.AccidentOrganizations).ThenInclude(ao => ao.Organization)
                      .Include(a => a.City),
                    cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.Accident>, IEnumerable<AccidentDto>>(query.Item2);

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
            var data = Mapper.Map<IEnumerable<Entities.Tracker.Accident>, IEnumerable<AccidentDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);

        }
        static Expression<Func<Entities.Tracker.Accident, bool>> PredicateBuilderFunction(AccidentFilter filter, string governorateId = null)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.Accident>(x => x.IsDeleted == filter.IsDeleted);

            if (!string.IsNullOrEmpty(filter.CityId))
            {
                predicate = predicate.And(x => x.CityId == filter.CityId);
            }
            if (filter.AccidentDate.HasValue)
            {
                predicate = predicate.And(x => x.AccidentDate == DateOnly.FromDateTime(filter.AccidentDate.Value));
            }
            if (!string.IsNullOrEmpty(filter.OrganizationId))
            {
                predicate = predicate.And(x => x.AccidentOrganizations.Any(ao => ao.OrganizationId == filter.OrganizationId));
            }
            if (!string.IsNullOrEmpty(filter.AccidentTypeId))
            {
                predicate = predicate.And(x => x.AccidentTypeId == filter.AccidentTypeId);
            }
            if (!string.IsNullOrEmpty(filter.FloatingUnitId))
            {
                predicate = predicate.And(x => x.FloatingUnitId == filter.FloatingUnitId);
            }
            if (filter.Case.HasValue)
            {
                predicate = predicate.And(x => x.Case == filter.Case.Value);
            }

            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x => x.GovernorateId == governorateId);
            }

            return predicate;
        }
        static Expression<Func<Entities.Tracker.Accident, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.Accident>(true);
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
        public override async Task<IFinalResult> AddAsync(AddAccidentDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var mapped = Mapper.Map<Entities.Tracker.Accident>(dto);
                mapped.GovernorateId = GetGovernorateIdFromClaims();
                SetEntityCreatedBaseProperties(mapped);

                var floatingUnit = await _floatingUnitService.GetByIdAsync(dto.FloatingUnitId, cancellationToken);

                string floatingUnitCode;

                if (floatingUnit.Data is FloatingUnitDto floatingUnitDto)
                {
                    floatingUnitCode = floatingUnitDto.Code;

                    AccidentFilter filter = new()
                    {
                        FloatingUnitId = dto.FloatingUnitId,
                        IsDeleted = false
                    };

                    // Check if the trip information already exists for the given floating unit id
                    var exist = await GetAllFilterAsync(filter, cancellationToken);
                    // Fix: Explicitly cast exist.Data to a collection type to access the Count property.
                    var existDataCollection = exist.Data as ICollection<AccidentDto>;

                    if (existDataCollection?.Count > 0 && 
                        DateTime.UtcNow.Day != 1 && 
                        DateTime.UtcNow.Month != 1)
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
                                mapped.Code = floatingUnitCode + number.ToString("D2"); // Ensure 4 digits
                            }
                        }
                    }
                    else
                    {
                        mapped.Code = floatingUnitCode + "01";
                    }
                }

                if (dto.Attach != null)
                {
                    foreach (var formFile in dto.Attach)
                    {
                        Guid guid = Guid.NewGuid();
                        var AddDto = new AddAttachDto
                        {
                            Id = guid.ToString(),
                            Path = formFile,
                            AttachType = "Accident",
                        };

                        IFinalResult Attach = await _attachService.AddAsync(AddDto, cancellationToken);

                        mapped.AccidentAttachments.Add(new Entities.Tracker.AccidentAttachment
                        {
                            AttachmentId = (string)Attach.Data,
                            AccidentId = mapped.Id
                        });
                    }
                }

                mapped.IsDeleted = false;

                UnitOfWork.Repository.Add(mapped);

                await UnitOfWork.SaveChangesAsync(cancellationToken);

                return ResponseResult.PostResult(mapped, status: HttpStatusCode.Created, message: HttpStatusCode.Created.ToString());
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.AddError + ex.Message);
            }

        }
        public override async Task<IFinalResult> UpdateAsync(AddAccidentDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, dto.Id);

                var newEntity = Mapper.Map(dto, entityToUpdate);
                newEntity.GovernorateId = GetGovernorateIdFromClaims();
                SetEntityModifiedBaseProperties(newEntity);

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        newEntity.IsDeleted = false;
                }

                if (entityToUpdate != null)
                {
                    if (dto.Attach != null)
                    {
                        foreach (var formFile in dto.Attach)
                        {
                            Guid guid = Guid.NewGuid();
                            var AddDto = new AddAttachDto
                            {
                                Id = guid.ToString(),
                                Path = formFile,
                                AttachType = "Accident",
                            };

                            IFinalResult Attach = await _attachService.AddAsync(AddDto, cancellationToken);

                            newEntity.AccidentAttachments.Add(new Entities.Tracker.AccidentAttachment
                            {
                                AttachmentId = (string)Attach.Data,
                                AccidentId = newEntity.Id
                            });
                        }
                    }
                }

                UnitOfWork.Repository.Update(entityToUpdate, newEntity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows > 0)
                {
                    Result = ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted,
                                            message: MessagesConstants.UpdateSuccess);
                }

                return Result;
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
                var entityToDelete = await UnitOfWork.Repository.GetAsync(cancellationToken, id);

                // Reomve Uploaded File
                _uploaderConfiguration.DeleteFile(entityToDelete.Attach);

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
        public async Task<IFinalResult> GetAllFilterAsync(AccidentFilter filter, CancellationToken cancellationToken = default)
        {
            var entity = await UnitOfWork.Repository.FindAsync(predicate: PredicateBuilderFunction(filter), cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.Accident>, IEnumerable<AccidentDto>>(entity.Where(x => x.IsDeleted != true));

            return ResponseResult.PostResult(data, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
    }
}
