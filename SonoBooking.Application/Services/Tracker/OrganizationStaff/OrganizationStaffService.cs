using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FuzzySharp;
using SonoTracker.Application.Services.Base;
using SonoTracker.Application.Services.Tracker.OrganizationStaffStaff;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.Organization;
using SonoTracker.Common.DTO.Tracker.OrganizationStaff;
using SonoTracker.Common.DTO.Tracker.OrganizationStaff.Parameters;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using SonoTracker.Application.Services.Tracker.Organizations;

namespace SonoTracker.Application.Services.Tracker.OrganizationStaff
{
    public class OrganizationStaffService : BaseService<Entities.Tracker.OrganizationStaff, AddOrganizationStaffDto, EditOrganizationStaffDto, OrganizationStaffDto, string, string>, IOrganizationStaffService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;

        private readonly IOrganizationService _organizationService;
        public OrganizationStaffService(
            IServiceBaseParameter<Entities.Tracker.OrganizationStaff> businessBaseParameter,
            IWebHostEnvironment hostingEnvironment, IHttpContextAccessor request,
            IOrganizationService organizationService) : base(businessBaseParameter)
        {
            _hostingEnvironment = hostingEnvironment;
            _request = request;
            _uploaderConfiguration = new UploaderConfiguration(_hostingEnvironment, _request);
            _organizationService = organizationService;
        }
        
        private static string NormalizeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // Normalize whitespace + case only (keep letters as-is for Arabic)
            var parts = value.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', parts).ToLowerInvariant();
        }

        private static string NormalizeNationalId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            // NationalId should be digits only for fuzzy/ratio comparisons
            return new string(value.Where(char.IsDigit).ToArray());
        }

        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr, include: src => src
                .Include(x => x.Organization), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Domain.Entities.Tracker.OrganizationStaff, EditOrganizationStaffDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr, 
                include: src => src.Include(t => t.Organization), cancellationToken: cancellationToken);

            var mapped = Mapper.Map<Domain.Entities.Tracker.OrganizationStaff, OrganizationStaffDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.OrganizationStaff, bool>> predicate = null, CancellationToken cancellationToken = default)
            => await GetAllAsync(organizationId: null, cancellationToken);

        public async Task<IFinalResult> GetAllAsync(string organizationId, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var filter = new OrganizationStaffFilter
            {
                OrganizationId = organizationId,
                // Non-superadmin always gets non-deleted rows
                // (for superadmin, the predicate bypasses IsDeleted entirely)
                IsDeleted = false
            };

            var entity = await UnitOfWork.Repository.FindAsync(
                predicate: PredicateBuilderFunction(filter, includeDeleted: isSuperAdmin),
                include: src => src
                    .Include(x => x.Organization),
                cancellationToken: cancellationToken);
            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.OrganizationStaff>, IEnumerable<OrganizationStaffDto>>(entity);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<OrganizationStaffFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var staffFilter = filter?.Filter ?? new OrganizationStaffFilter();
            if (!isSuperAdmin)
                staffFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(staffFilter, includeDeleted: false), pageNumber: offset, pageSize: limit, filter.OrderByValue,
                include: src => src
             .Include(x => x.Organization),
                cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.OrganizationStaff>, IEnumerable<OrganizationStaffDto>>(query.Item2);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<PagingResult> GetDropDownAsync(BaseParam<SearchCriteriaFilter> filter, CancellationToken cancellationToken = default)
        {
            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var isSuperAdmin = IsSuperAdmin();
            var predicate = DropDownPredicateBuilderFunction(filter.Filter);

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: predicate, pageNumber: offset, pageSize: limit, cancellationToken: cancellationToken);

            var items = isSuperAdmin ? query.Item2 : query.Item2.Where(x => x.IsDeleted != true);
            var data = Mapper.Map<IEnumerable<Entities.Tracker.OrganizationStaff>, IEnumerable<OrganizationStaffDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);

        }


        static Expression<Func<Entities.Tracker.OrganizationStaff, bool>> PredicateBuilderFunction(OrganizationStaffFilter filter, bool includeDeleted)
        {
            var predicate = includeDeleted
                ? PredicateBuilder.New<Entities.Tracker.OrganizationStaff>(true)
                : PredicateBuilder.New<Entities.Tracker.OrganizationStaff>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                predicate = predicate.And(x => x.Name.Contains(filter.Name));
            }
            if (!string.IsNullOrWhiteSpace(filter.Job))
            {
                predicate = predicate.And(x => x.Job.Contains(filter.Job));
            }
            if (!string.IsNullOrEmpty(filter.OrganizationId))
            {
                predicate = predicate.And(x => x.OrganizationId == filter.OrganizationId);
            }
            if (filter.IsDelegate)
            {
                predicate = predicate.And(x => x.IsDelegate == filter.IsDelegate);
            }

            return predicate;
        }
        static Expression<Func<Entities.Tracker.OrganizationStaff, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.OrganizationStaff>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.Name.Contains(filter.SearchCriteria));
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
        public override async Task<IFinalResult> AddAsync(AddOrganizationStaffDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                // Duplicate guard using fuzzy matching (Name) + exact/fuzzy matching (NationalId)
                var existingStaff = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.OrganizationId == model.OrganizationId,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                var normalizedName = NormalizeText(model.Name);
                var normalizedNationalId = NormalizeNationalId(model.NationalId);

                const int nameThreshold = 90;
                const int nationalIdFuzzyThreshold = 90;

                var nameDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Name) &&
                    Fuzz.TokenSetRatio(normalizedName, NormalizeText(x.Name)) >= nameThreshold);

                var nationalIdExactDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.NationalId) &&
                    string.Equals(x.NationalId, model.NationalId, StringComparison.Ordinal));

                var nationalIdFuzzyDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.NationalId) &&
                    Fuzz.Ratio(normalizedNationalId, NormalizeNationalId(x.NationalId)) >= nationalIdFuzzyThreshold);

                if (nameDuplicate || nationalIdExactDuplicate || nationalIdFuzzyDuplicate)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null, message: MessagesConstants.Existed);

                Entities.Tracker.OrganizationStaff entity = Mapper.Map<Entities.Tracker.OrganizationStaff>(model);

                if (model.DelegateAttachment != null)
                {
                    IFinalResult organizationResult = await _organizationService.GetByIdAsync(model.OrganizationId, cancellationToken);
                    
                    if (organizationResult?.Data is not OrganizationDto organization)
                        return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                         message: MessagesConstants.NotFound);

                    string res = await _uploaderConfiguration
                                       .UploadFile(model.DelegateAttachment, $"OrganizationStaff/{organization.OrganizationType}", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    entity.DelegateAttachment = res;

                    entity.IsDelegate = true;
                }
                else
                {
                    entity.IsDelegate = false;
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
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: ex,
                       message: MessagesConstants.AddError + ex.Message);
            } 
        }
        public override async Task<IFinalResult> UpdateAsync(AddOrganizationStaffDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                Entities.Tracker.OrganizationStaff entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                string currentDelegateAttachment = entityToUpdate.DelegateAttachment;

                var entity = Mapper.Map(model, entityToUpdate);

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        entity.IsDeleted = false;
                }

                // Duplicate guard (exclude current Id)
                var existingStaff = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.OrganizationId == model.OrganizationId && x.Id != model.Id,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                var normalizedName = NormalizeText(model.Name);
                var normalizedNationalId = NormalizeNationalId(model.NationalId);

                const int nameThreshold = 90;
                const int nationalIdFuzzyThreshold = 90;

                var nameDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Name) &&
                    Fuzz.TokenSetRatio(normalizedName, NormalizeText(x.Name)) >= nameThreshold);

                var nationalIdExactDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.NationalId) &&
                    string.Equals(x.NationalId, model.NationalId, StringComparison.Ordinal));

                var nationalIdFuzzyDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.NationalId) &&
                    Fuzz.Ratio(normalizedNationalId, NormalizeNationalId(x.NationalId)) >= nationalIdFuzzyThreshold);

                if (nameDuplicate || nationalIdExactDuplicate || nationalIdFuzzyDuplicate)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null, 
                                                     message: MessagesConstants.Existed);

                if (model.DelegateAttachment != null)
                {
                    IFinalResult organizationResult = await _organizationService.GetByIdAsync(model.OrganizationId, cancellationToken);

                    if (organizationResult?.Data is not OrganizationDto organization)
                    {
                        return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                         message: MessagesConstants.NotFound);
                    }

                    string res = await _uploaderConfiguration
                                   .UploadFile(model.DelegateAttachment, $"OrganizationStaff/{organization.OrganizationType}", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    _uploaderConfiguration.DeleteFile(entityToUpdate.DelegateAttachment);

                    entity.DelegateAttachment = res;

                    entity.IsDelegate = true;
                }
                else
                {
                    entity.DelegateAttachment = currentDelegateAttachment;
                }

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
        public override async Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entityToDelete = await UnitOfWork.Repository.GetAsync(cancellationToken, id);

                // Remove Uploaded File
                _uploaderConfiguration.DeleteFile(entityToDelete.DelegateAttachment);

                UnitOfWork.Repository.Remove(entityToDelete);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                                                     message: MessagesConstants.DeleteError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                                                 message: MessagesConstants.DeleteSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: ex,
                                                 message: MessagesConstants.DeleteError + ex.Message);
                //_logger.LogError($"{MessagesConstants.DeleteError}-{nameof(DeleteAsync)}");
                //_logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                throw;
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
