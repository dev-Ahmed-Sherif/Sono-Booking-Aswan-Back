using Microsoft.EntityFrameworkCore;
using SonoBooking.Application.Services.Base;
using SonoBooking.Common.Constants;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Request;
using SonoBooking.Common.DTO.Housing.Request.Parameters;
using SonoBooking.Common.DTO.Housing.RequestParticipant;
using SonoBooking.Common.DTO.Housing.RequestUnit;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Requests
{
    public class RequestService(IServiceBaseParameter<Request> businessBaseParameter) : BaseService<Request, AddRequestDto, EditRequestDto, RequestDto, string, string>(businessBaseParameter), IRequestService
    {
        public override async Task<IFinalResult> GetAllAsync(
            bool disableTracking = false,
            Expression<Func<Request, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
            string userId = GetUserIdFromHeader();

            IEnumerable<Request> query;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                string trimmedUserId = userId.Trim();
                query = await UnitOfWork.Repository.FindAsync(
                    x => x.UserId == trimmedUserId,
                    include: src => src.Include(r => r.RequestType),
                    disableTracking: disableTracking,
                    cancellationToken: cancellationToken);
            }
            else if (predicate != null)
            {
                query = await UnitOfWork.Repository.FindAsync(
                    predicate,
                    include: src => src.Include(r => r.RequestType),
                    disableTracking: disableTracking,
                    cancellationToken: cancellationToken);
            }
            else
            {
                query = await UnitOfWork.Repository.GetAllAsync(
                    include: src => src.Include(r => r.RequestType),
                    disableTracking: disableTracking,
                    cancellationToken: cancellationToken);
            }

            IEnumerable<RequestDto> data = Mapper.Map<IEnumerable<Request>, IEnumerable<RequestDto>>(query);
            IEnumerable<RequestDto> sorted = data
                .OrderBy(r => RequestTypeIds.GetSortOrder(r.RequestTypeId))
                .ThenByDescending(r => r.CreatedAt);

            return ResponseResult.PostResult(result: sorted, status: HttpStatusCode.OK, exception: null,
                message: MessagesConstants.Success);
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<RequestFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            RequestFilter requestFilter = filter?.Filter ?? new RequestFilter();

            (int Count, IEnumerable<Request> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == requestFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<RequestDto> data = Mapper.Map<IEnumerable<Request>, IEnumerable<RequestDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public override async Task<IFinalResult> AddAsync(AddRequestDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                //IFinalResult validation = ValidateRequestDto(model);
                //if (validation != null)
                //    return validation;

                Request entity = Mapper.Map<Request>(model);
                entity.RequestNumber = await GenerateRequestNumberAsync(cancellationToken);
                entity.RequestDate = DateTime.UtcNow;
                entity.Status = Status.Pending;
                entity.UserId = !string.IsNullOrWhiteSpace(_user.Id) ? _user.Id : entity.CreatedById;

                SetEntityCreatedBaseProperties(entity);

                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);
                await SyncRequestUnitsAsync(entity.Id, model.RequestUnits, [], cancellationToken);
                await SyncRequestParticipantsAsync(entity.Id, model.RequestCompanions, [], cancellationToken);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.AddError);

                return ResponseResult.PostResult(result: entity.Id, status: HttpStatusCode.Created, exception: null,
                    message: MessagesConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                string detail = ex.InnerException?.Message ?? ex.Message;
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: ex,
                    message: string.IsNullOrWhiteSpace(detail) ? MessagesConstants.AddError : detail);
            }
        }

        public override async Task<IFinalResult> UpdateAsync(AddRequestDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                //IFinalResult validation = ValidateRequestDto(model);
                //if (validation != null)
                //    return validation;

                Request entityToUpdate = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id.Equals(model.Id),
                    include: src => src
                        .Include(r => r.RequestUnits)
                        .Include(r => r.RequestParticipants),
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToUpdate == null)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.NotFound, exception: null,
                        message: MessagesConstants.NotFound);

                Request entity = Mapper.Map(model, entityToUpdate);

                entity.RequestNumber = entityToUpdate.RequestNumber;
                entity.RequestDate = entityToUpdate.RequestDate;
                entity.UserId = entityToUpdate.UserId;
                entity.Status = model.Status ?? entityToUpdate.Status;

                if (IsSuperAdmin())
                    entity.IsDeleted = false;

                SetEntityModifiedBaseProperties(entity);

                UnitOfWork.Repository.Update(entityToUpdate, entity);

                await SyncRequestUnitsAsync(entity.Id, model.RequestUnits, entityToUpdate.RequestUnits.ToList(), cancellationToken);
                await SyncRequestParticipantsAsync(entity.Id, model.RequestCompanions, entityToUpdate.RequestParticipants.ToList(), cancellationToken);

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
                Request entityToDelete = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id.Equals(id),
                    include: src => src
                        .Include(r => r.RequestParticipants)
                        .Include(r => r.RequestUnits)
                        .Include(r => r.Approval)
                        .Include(r => r.Reservation).ThenInclude(res => res.Payment),
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToDelete.Reservation?.Payment != null)
                    UnitOfWork.GetRepository<Payment>().Remove(entityToDelete.Reservation.Payment);

                if (entityToDelete.Reservation != null)
                    UnitOfWork.GetRepository<Reservation>().Remove(entityToDelete.Reservation);
                UnitOfWork.GetRepository<Approval>().Remove(entityToDelete.Approval);
                UnitOfWork.GetRepository<RequestUnit>().RemoveRange(entityToDelete.RequestUnits, cancellationToken);
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

        private async Task<string> GenerateRequestNumberAsync(CancellationToken cancellationToken)
        {
            int year = DateTime.UtcNow.Year;
            string prefix = $"REQ-{year}-";
            int sequence = await UnitOfWork.Repository.Count(
                r => r.RequestNumber.StartsWith(prefix),
                cancellationToken) + 1;

            return $"{prefix}{sequence:D4}";
        }

        private string GetUserIdFromHeader() =>
            HttpContextAccessor?.HttpContext?.Request.Headers["UserId"].FirstOrDefault()?.Trim();

        private IFinalResult ValidateRequestDto(AddRequestDto model)
        {
            if (model.Nights <= 0)
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "Nights must be greater than zero.");

            if (string.IsNullOrWhiteSpace(model.RequestTypeId))
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "RequestTypeId is required.");

            if (model.RequestUnits != null)
            {
                foreach (AddRequestUnitDto unit in model.RequestUnits)
                {
                    if (string.IsNullOrWhiteSpace(unit.ApartmentId))
                        return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                            message: "RequestUnits.ApartmentId is required for each selected unit.");
                }
            }

            return null;
        }

        private async Task SyncRequestUnitsAsync(
            string requestId,
            ICollection<AddRequestUnitDto> units,
            List<RequestUnit> existing,
            CancellationToken cancellationToken)
        {
            if (existing.Count > 0)
                UnitOfWork.GetRepository<RequestUnit>().RemoveRange(existing, cancellationToken);

            if (units == null || units.Count == 0)
                return;

            foreach (AddRequestUnitDto dto in units)
            {
                RequestUnit unit = Mapper.Map<RequestUnit>(dto);
                unit.RequestId = requestId;
                await UnitOfWork.GetRepository<RequestUnit>().AddAsync(unit, cancellationToken);
            }
        }

        private async Task SyncRequestParticipantsAsync(
            string requestId,
            ICollection<AddRequestParticipantDto> companions,
            List<RequestParticipant> existing,
            CancellationToken cancellationToken)
        {
            if (existing.Count > 0)
                UnitOfWork.GetRepository<RequestParticipant>().RemoveRange(existing, cancellationToken);

            if (companions == null || companions.Count == 0)
                return;

            foreach (AddRequestParticipantDto dto in companions)
            {
                RequestParticipant participant = Mapper.Map<RequestParticipant>(dto);
                participant.RequestId = requestId;
                await UnitOfWork.GetRepository<RequestParticipant>().AddAsync(participant, cancellationToken);
            }
        }
    }
}
