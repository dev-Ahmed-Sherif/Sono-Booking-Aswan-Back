using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using FuzzySharp;
using SonoTracker.Application.Services.Base;
using SonoTracker.Common.Core;
using SonoTracker.Common.DTO.Base;
using SonoTracker.Common.DTO.Tracker.FloatingUnit;
using SonoTracker.Common.DTO.Tracker.FloatingUnitStaff;
using SonoTracker.Common.DTO.Tracker.FloatingUnitStaff.Parameters;
using SonoTracker.Common.DTO.Tracker.OrganizationStaff;
using SonoTracker.Common.Helpers.MediaUploader;
using SonoTracker.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoTracker.Application.Services.Tracker.FloatingUnitStaff
{
    public class FloatingUnitStaffService : BaseService<Entities.Tracker.FloatingUnitStaff, AddFloatingUnitStaffDto, EditFloatingUnitStaffDto, FloatingUnitStaffDto, string, string>, IFloatingUnitStaffService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _request;
        private readonly UploaderConfiguration _uploaderConfiguration;
        public FloatingUnitStaffService(IServiceBaseParameter<Entities.Tracker.FloatingUnitStaff> businessBaseParameter,
            IWebHostEnvironment hostingEnvironment, IHttpContextAccessor request) : base(businessBaseParameter)
        {
            _hostingEnvironment = hostingEnvironment;
            _request = request;
            _uploaderConfiguration = new UploaderConfiguration(_hostingEnvironment, _request);
        }

        private static string NormalizeText(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var parts = value.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', parts).ToLowerInvariant();
        }

        private static string NormalizeIdentity(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return new string(value.Where(char.IsDigit).ToArray());
        }


        public override async Task<IFinalResult> GetByIdForEditAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr, include: src => src
            
                .Include(t => t.Nationality), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.FloatingUnitStaff, EditFloatingUnitStaffDto>(entity);
            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            var idStr = id?.ToString();
            var entity = await UnitOfWork.Repository.FirstOrDefaultAsync(x => x.Id == idStr, include: src => src
                .Include(t => t.Nationality), cancellationToken: cancellationToken);
            var mapped = Mapper.Map<Entities.Tracker.FloatingUnitStaff, EditFloatingUnitStaffDto>(entity);

            return ResponseResult.PostResult(mapped, HttpStatusCode.OK);
        }

        public override async Task<IFinalResult> GetAllAsync(bool disableTracking = false, Expression<Func<Entities.Tracker.FloatingUnitStaff, bool>> predicate = null, CancellationToken cancellationToken = default)
            => await GetAllAsync(floatingUnitId: null, cancellationToken);

        public async Task<IFinalResult> GetAllAsync(string? floatingUnitId, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var filter = new FloatingUnitStaffFilter
            {
                FloatingUnitId = floatingUnitId,
                IsDeleted = false
            };
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            var entity = await UnitOfWork.Repository.FindAsync(
                predicate: PredicateBuilderFunction(filter, governorateId, includeDeleted: isSuperAdmin),
                include: src => src
                    .Include(t => t.Nationality),
                cancellationToken: cancellationToken);
            var mapped = Mapper.Map<IEnumerable<Entities.Tracker.FloatingUnitStaff>, IEnumerable<FloatingUnitStaffDto>>(entity);
            return ResponseResult.PostResult(mapped, status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<FloatingUnitStaffFilter> filter, CancellationToken cancellationToken = default)
        {
            var isSuperAdmin = IsSuperAdmin();
            var floatingUnitStaffFilter = filter?.Filter ?? new FloatingUnitStaffFilter();
            var governorateId = isSuperAdmin ? null : GetGovernorateIdFromClaims();

            if (!isSuperAdmin)
                floatingUnitStaffFilter.IsDeleted = false;

            var limit = filter.PageSize;

            var offset = --filter.PageNumber * filter.PageSize;

            var query = await UnitOfWork.Repository.FindPagedAsync(predicate: PredicateBuilderFunction(floatingUnitStaffFilter, governorateId, includeDeleted: false), pageNumber: offset, pageSize: limit, filter.OrderByValue, cancellationToken: cancellationToken);

            var data = Mapper.Map<IEnumerable<Entities.Tracker.FloatingUnitStaff>, IEnumerable<FloatingUnitStaffDto>>(query.Item2);

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
            var data = Mapper.Map<IEnumerable<Entities.Tracker.FloatingUnitStaff>, IEnumerable<FloatingUnitStaffDto>>(items);

            return new PagingResult(filter.PageNumber, filter.PageSize, query.Item1, data, status: HttpStatusCode.OK, MessagesConstants.Success);

        }

        static Expression<Func<Entities.Tracker.FloatingUnitStaff, bool>> PredicateBuilderFunction(FloatingUnitStaffFilter filter, string governorateId = null, bool includeDeleted = false)
        {
            var predicate = includeDeleted
                ? PredicateBuilder.New<Entities.Tracker.FloatingUnitStaff>(true)
                : PredicateBuilder.New<Entities.Tracker.FloatingUnitStaff>(x => x.IsDeleted == filter.IsDeleted);
            if (!string.IsNullOrWhiteSpace(filter.Name))
            {
                predicate = predicate.And(x => x.Name.Contains(filter.Name));
            }
            if (!string.IsNullOrWhiteSpace(filter.Mobile))
            {
                predicate = predicate.And(x => x.Mobile.Contains(filter.Mobile));
            }
            if (!string.IsNullOrWhiteSpace(filter.Email))
            {
                predicate = predicate.And(x => x.Email.Contains(filter.Email));
            }
            if (!string.IsNullOrWhiteSpace(filter.Identity))
            {
                predicate = predicate.And(x => x.Identity.Contains(filter.Identity));
            }
            if (filter.gender.HasValue)
            {
                predicate = predicate.And(x => x.Gender == filter.gender.Value);
            }
            if (filter.IDType.HasValue)
            {
                predicate = predicate.And(x => x.IDType == filter.IDType.Value);
            }
            if (!string.IsNullOrEmpty(filter.FloatingUnitId))
            {
                predicate = predicate.And(x => x.FloatingUnitId == filter.FloatingUnitId);
            }
            if (!string.IsNullOrEmpty(filter.NationalityId))
            {
                predicate = predicate.And(x => x.NationalityId == filter.NationalityId);
            }
            if (filter.IsDelegate.HasValue)
            {
                predicate = predicate.And(x => x.IsDelegate == filter.IsDelegate.Value);
            }
            if (!string.IsNullOrWhiteSpace(governorateId))
            {
                predicate = predicate.And(x => x.GovernorateId == governorateId);
            }

            return predicate;
        }
       
        static Expression<Func<Entities.Tracker.FloatingUnitStaff, bool>> DropDownPredicateBuilderFunction(SearchCriteriaFilter filter)
        {
            var predicate = PredicateBuilder.New<Entities.Tracker.FloatingUnitStaff>(true);
            if (!string.IsNullOrWhiteSpace(filter?.SearchCriteria))
            {
                predicate = predicate.And(b => b.Name.Contains(filter.SearchCriteria));
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

        public override async Task<IFinalResult> AddAsync(AddFloatingUnitStaffDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                var existingStaff = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.FloatingUnitId == model.FloatingUnitId,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                var normalizedName = NormalizeText(model.Name);
                var normalizedIdentity = NormalizeIdentity(model.Identity);
                const int nameThreshold = 90;
                const int identityThreshold = 90;

                var nameDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Name) &&
                    Fuzz.TokenSetRatio(normalizedName, NormalizeText(x.Name)) >= nameThreshold);

                var identityExactDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Identity) &&
                    string.Equals(x.Identity, model.Identity, StringComparison.Ordinal));

                var identityFuzzyDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Identity) &&
                    Fuzz.Ratio(normalizedIdentity, NormalizeIdentity(x.Identity)) >= identityThreshold);

                if (nameDuplicate || identityExactDuplicate || identityFuzzyDuplicate)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                var entity = Mapper.Map<Entities.Tracker.FloatingUnitStaff>(model);

                if (model.DelegateAttachment != null)
                {
                    string res = await _uploaderConfiguration
                                       .UploadFile(model.DelegateAttachment, "FloatingUnitStaff", cancellationToken);

                    if (res != null)
                    {
                        if (UploadResponse(res) != null)
                            return UploadResponse(res);
                    }

                    entity.DelegateAttachment = res;

                    entity.IsDelegate = true;
                }

                entity.IsDeleted = false;

                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0) return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.AddError);

                return ResponseResult.PostResult(result: entity.Id, HttpStatusCode.Created, null, MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.AddError + ex.Message);
            }
        }
        public override async Task<IFinalResult> UpdateAsync(AddFloatingUnitStaffDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                Entities.Tracker.FloatingUnitStaff entityToUpdate = await UnitOfWork.Repository.GetAsync(cancellationToken, model.Id);

                string currentAttachment = entityToUpdate.DelegateAttachment;

                var entity = Mapper.Map(model, entityToUpdate);
                entity.GovernorateId = GetGovernorateIdFromClaims();

                if (IsSuperAdmin())
                {
                    if (entityToUpdate.IsDeleted)
                        entity.IsDeleted = false;
                }

                var existingStaff = await UnitOfWork.Repository.FindAsync(
                    predicate: x => x.FloatingUnitId == model.FloatingUnitId && x.Id != model.Id,
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                var normalizedName = NormalizeText(model.Name);
                var normalizedIdentity = NormalizeIdentity(model.Identity);
                const int nameThreshold = 90;
                const int identityThreshold = 90;

                var nameDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Name) &&
                    Fuzz.TokenSetRatio(normalizedName, NormalizeText(x.Name)) >= nameThreshold);

                var identityExactDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Identity) &&
                    string.Equals(x.Identity, model.Identity, StringComparison.Ordinal));

                var identityFuzzyDuplicate = existingStaff.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Identity) &&
                    Fuzz.Ratio(normalizedIdentity, NormalizeIdentity(x.Identity)) >= identityThreshold);

                if (nameDuplicate || identityExactDuplicate || identityFuzzyDuplicate)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                                                     message: MessagesConstants.Existed);

                if (model.DelegateAttachment != null)
                {
                    string res = await _uploaderConfiguration
                                   .UploadFile(model.DelegateAttachment, "FloatingUnitStaff", cancellationToken);

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
                    entity.DelegateAttachment = currentAttachment;
                }

                UnitOfWork.Repository.Update(entityToUpdate, entity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0) return ResponseResult.PostResult(false, HttpStatusCode.BadRequest, null, MessagesConstants.UpdateError);

                return ResponseResult.PostResult(true, HttpStatusCode.OK, null, MessagesConstants.UpdateSuccess);
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

                // Remove Uploaded File
                _uploaderConfiguration.DeleteFile(entityToDelete.DelegateAttachment);

                UnitOfWork.Repository.Remove(entityToDelete);
                
                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);
                
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
                                                 message: MessagesConstants.DeleteError);
                //_logger.LogError($"{MessagesConstants.DeleteError}-{nameof(DeleteAsync)}");
                //_logger.LogError(JsonConvert.SerializeObject(e, _serializerSettings));
                //throw;
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
