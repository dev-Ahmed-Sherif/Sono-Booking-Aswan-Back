using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Companion;
using SonoBooking.Common.DTO.Housing.Companion.Parameters;
using SonoBooking.Common.Helpers.MediaUploader;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Companions
{
    public class CompanionService(
        IServiceBaseParameter<Companion> businessBaseParameter,
        IWebHostEnvironment hostingEnvironment,
        IHttpContextAccessor httpContextAccessor)
        : BaseService<Companion, AddCompanionDto, EditCompanionDto, CompanionDto, string, string>(businessBaseParameter), ICompanionService
    {
        private readonly UploaderConfiguration _uploaderConfiguration = new(hostingEnvironment, httpContextAccessor);
        public async Task<PagingResult> GetAllPagedAsync(BaseParam<CompanionFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            CompanionFilter companionFilter = filter?.Filter ?? new CompanionFilter();

            (int Count, IEnumerable<Companion> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == companionFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<CompanionDto> data = Mapper.Map<IEnumerable<Companion>, IEnumerable<CompanionDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            Companion query = await UnitOfWork.Repository.GetAsync(cancellationToken, id);

            if (query == null)
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.NotFound, exception: null,
                    message: MessagesConstants.NotFound);

            if (!CanAccessCompanion(query))
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.Unauthorized, exception: null,
                    message: "UserId is required.");

            CompanionDto data = Mapper.Map<Companion, CompanionDto>(query);
            return ResponseResult.PostResult(result: data, status: HttpStatusCode.OK, exception: null,
                message: MessagesConstants.Success);
        }

        public override async Task<IFinalResult> GetAllAsync(
            bool disableTracking = false,
            Expression<Func<Companion, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
            string ownerUserId = ResolveOwnerUserIdForRead();
            if (!IsAuthenticatedUser() && string.IsNullOrWhiteSpace(ownerUserId))
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.Unauthorized, exception: null,
                    message: "UserId is required.");

            if (!string.IsNullOrWhiteSpace(ownerUserId) && predicate == null)
                predicate = x => x.UserId == ownerUserId;

            return await base.GetAllAsync(disableTracking, predicate, cancellationToken);
        }

        public override async Task<IFinalResult> AddAsync(AddCompanionDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                string ownerUserId = ResolveOwnerUserId(model);
                if (string.IsNullOrWhiteSpace(ownerUserId))
                    return ResponseResult.PostResult(result: null, status: HttpStatusCode.Unauthorized, exception: null,
                        message: "UserId is required.");

                if (await HasDuplicateRelationshipAsync(ownerUserId, model.RelationshipId, cancellationToken: cancellationToken))
                    return ResponseResult.PostResult(result: null, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                if (await HasDuplicateDocumentNumberAsync(ownerUserId, model.DocumentNumber, cancellationToken: cancellationToken))
                    return ResponseResult.PostResult(result: null, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                if (model.DocumentImage == null)
                    return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                        message: "Document image is required.");

                string documentUpload = await _uploaderConfiguration.UploadFile(model.DocumentImage, "Attach/Companions", cancellationToken);
                if (UploadResponse(documentUpload) is { } uploadErr)
                    return uploadErr;

                Companion entity = Mapper.Map<Companion>(model);

                entity.UserId = ownerUserId;
                entity.DocumentImageUrl = documentUpload;

                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.AddError);

                return ResponseResult.PostResult(result: entity.Id, status: HttpStatusCode.Created, exception: null,
                    message: MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: MessagesConstants.AddError);
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddCompanionDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                Companion entityToUpdate = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id.Equals(model.Id),
                    include: null,
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToUpdate == null)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.NotFound, exception: null,
                        message: MessagesConstants.NotFound);

                if (!CanAccessCompanion(entityToUpdate, model))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Unauthorized, exception: null,
                        message: "UserId is required.");

                if (await HasDuplicateRelationshipAsync(entityToUpdate.UserId, model.RelationshipId, model.Id, cancellationToken))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                if (await HasDuplicateDocumentNumberAsync(entityToUpdate.UserId, model.DocumentNumber, model.Id, cancellationToken))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Conflict, exception: null,
                        message: MessagesConstants.Existed);

                if (model.DocumentImage != null)
                {
                    string documentUpload = await _uploaderConfiguration.UploadFile(model.DocumentImage, "Attach/Companions", cancellationToken);
                    if (UploadResponse(documentUpload) is { } uploadErr)
                        return uploadErr;

                    _uploaderConfiguration.DeleteFile(entityToUpdate.DocumentImageUrl);
                    entityToUpdate.DocumentImageUrl = documentUpload;
                }

                Companion entity = Mapper.Map(model, entityToUpdate);

                entity.UserId = entityToUpdate.UserId;

                if (IsSuperAdmin())
                    entity.IsDeleted = false;

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
                    message: ex.Message);
            }
        }

        public override async Task<IFinalResult> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                Companion entityToDelete = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id.Equals(id),
                    include: src => src.Include(c => c.RequestParticipants),
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToDelete == null)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.NotFound, exception: null,
                        message: MessagesConstants.NotFound);

                if (!CanAccessCompanion(entityToDelete))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Unauthorized, exception: null,
                        message: "UserId is required.");

                _uploaderConfiguration.DeleteFile(entityToDelete.DocumentImageUrl);

                if (entityToDelete.RequestParticipants.Count > 0)
                    UnitOfWork.GetRepository<RequestParticipant>().RemoveRange(entityToDelete.RequestParticipants, cancellationToken);

                UnitOfWork.Repository.Remove(entityToDelete);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.DeleteError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                    message: MessagesConstants.DeleteSuccess);
            }
            catch (Exception e)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: e,
                    message: MessagesConstants.DeleteError);
            }
        }

        public override async Task<IFinalResult> DeleteSoftAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                Companion entityToDelete = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id.Equals(id),
                    include: null,
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToDelete == null)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.NotFound, exception: null,
                        message: MessagesConstants.NotFound);

                if (!CanAccessCompanion(entityToDelete))
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.Unauthorized, exception: null,
                        message: "UserId is required.");

                UnitOfWork.Repository.RemoveLogical(entityToDelete);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.DeleteError);

                return ResponseResult.PostResult(result: true, status: HttpStatusCode.Accepted, exception: null,
                    message: MessagesConstants.DeleteSuccess);
            }
            catch (Exception e)
            {
                return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: e,
                    message: MessagesConstants.DeleteError);
            }
        }

        private bool IsAuthenticatedUser() => !string.IsNullOrWhiteSpace(_user?.Id);

        private string GetRegistrationUserIdFromHeader() =>
            HttpContextAccessor?.HttpContext?.Request.Headers["UserId"].FirstOrDefault()?.Trim();

        private string ResolveOwnerUserId(AddCompanionDto model)
        {
            if (IsAuthenticatedUser())
                return _user.Id;

            return !string.IsNullOrWhiteSpace(model?.UserId)
                ? model.UserId.Trim()
                : GetRegistrationUserIdFromHeader();
        }

        private string ResolveOwnerUserIdForRead()
        {
            //if (IsAuthenticatedUser())
            //    return IsSuperAdmin() ? null : _user.Id;

            return GetRegistrationUserIdFromHeader();
        }

        private bool CanAccessCompanion(Companion companion, AddCompanionDto model = null)
        {
            if (IsAuthenticatedUser())
                return IsSuperAdmin() || string.Equals(companion.UserId, _user.Id, StringComparison.Ordinal);

            string registrationUserId = !string.IsNullOrWhiteSpace(model?.UserId)
                ? model.UserId.Trim()
                : GetRegistrationUserIdFromHeader();

            return !string.IsNullOrWhiteSpace(registrationUserId) &&
                   string.Equals(companion.UserId, registrationUserId, StringComparison.Ordinal);
        }

        private async Task<bool> HasDuplicateRelationshipAsync(
            string userId,
            string relationshipId,
            string excludeCompanionId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(relationshipId))
                return false;

            if (!RelationshipIds.IsUniquePerUser(relationshipId))
                return false;

            return await UnitOfWork.Repository.Any(
                c => c.UserId == userId
                    && c.RelationshipId == relationshipId
                    && !c.IsDeleted
                    && (excludeCompanionId == null || c.Id != excludeCompanionId),
                cancellationToken);
        }

        private async Task<bool> HasDuplicateDocumentNumberAsync(
            string userId,
            string documentNumber,
            string excludeCompanionId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(documentNumber))
                return false;

            string normalizedDocumentNumber = documentNumber.Trim();

            return await UnitOfWork.Repository.Any(
                c => c.UserId == userId
                    && c.DocumentNumber == normalizedDocumentNumber
                    && !c.IsDeleted
                    && (excludeCompanionId == null || c.Id != excludeCompanionId),
                cancellationToken);
        }

        private IFinalResult UploadResponse(string res)
        {
            if (res == "Size")
            {
                const string message = "File Size Larger than 5 Mega Bytes";
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null, message: message);
            }

            if (res == "Type")
            {
                const string message = "File type not allowed.";
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null, message: message);
            }

            return null;
        }
    }
}
