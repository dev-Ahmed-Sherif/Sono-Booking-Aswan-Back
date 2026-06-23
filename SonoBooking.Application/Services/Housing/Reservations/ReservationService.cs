using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using SonoBooking.Application.Services.Base;
using SonoBooking.Application.Services.Housing.Notifications;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Reservation;
using SonoBooking.Common.DTO.Housing.Reservation.Parameters;
using SonoBooking.Common.DTO.Reports.Reservations;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Reservations
{
    public class ReservationService(
        IServiceBaseParameter<Reservation> businessBaseParameter,
        ReservationStatusEmailNotifier statusEmailNotifier,
        HousingNotificationService housingNotificationService) : BaseService<Reservation, AddReservationDto, EditReservationDto, ReservationDto, string, string>(businessBaseParameter), IReservationService
    {
        public override async Task<IFinalResult> GetAllAsync(
            bool disableTracking = false,
            Expression<Func<Reservation, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
            string userId = GetUserIdFromHeader();

            IEnumerable<Reservation> query;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                string trimmedUserId = userId.Trim();
                query = await UnitOfWork.Repository.FindAsync(
                    x => x.Request.UserId == trimmedUserId,
                    include: src => src.Include(r => r.Request),
                    disableTracking: disableTracking,
                    cancellationToken: cancellationToken);
            }
            else if (predicate != null)
            {
                query = await UnitOfWork.Repository.FindAsync(
                    predicate,
                    include: src => src.Include(r => r.Request),
                    disableTracking: disableTracking,
                    cancellationToken: cancellationToken);
            }
            else
            {
                query = await UnitOfWork.Repository.GetAllAsync(
                    include: src => src.Include(r => r.Request),
                    disableTracking: disableTracking,
                    cancellationToken: cancellationToken);
            }

            IEnumerable<ReservationDto> data = Mapper.Map<IEnumerable<Reservation>, IEnumerable<ReservationDto>>(query);
            IEnumerable<ReservationDto> sorted = data.OrderByDescending(r => r.CreatedAt);

            return ResponseResult.PostResult(result: sorted, status: HttpStatusCode.OK, exception: null,
                message: MessagesConstants.Success);
        }

        public async Task<PagingResult> GetAllPagedAsync(BaseParam<ReservationFilter> filter, CancellationToken cancellationToken = default)
        {
            int pageNumber = filter.PageNumber;
            int limit = filter.PageSize;
            int offset = (pageNumber - 1) * limit;
            ReservationFilter reservationFilter = filter?.Filter ?? new ReservationFilter();

            (int Count, IEnumerable<Reservation> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == reservationFilter.IsDeleted,
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                cancellationToken: cancellationToken);

            IEnumerable<ReservationDto> data = Mapper.Map<IEnumerable<Reservation>, IEnumerable<ReservationDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }

        public async Task<IFinalResult> GetAllReportAsync(FilterReservationReportDto filter, CancellationToken cancellationToken = default)
        {
            IEnumerable<Reservation> query = await UnitOfWork.Repository.FindAsync(
                predicate: PredicateBuilderReportFunction(filter),
                include: src => src
                    .Include(r => r.Request).ThenInclude(req => req.User)
                    .Include(r => r.Request).ThenInclude(req => req.RequestUnits)
                    .Include(r => r.Payment),
                cancellationToken: cancellationToken);

            List<ReservationReportDto> reportData = Mapper.Map<List<ReservationReportDto>>(query);
            string startDateReport = filter.StartDate.ToString("dd/MM/yyyy");
            string endDateReport = filter.EndDate.ToString("dd/MM/yyyy");

            foreach (ReservationReportDto row in reportData)
            {
                row.StartDateReport = startDateReport;
                row.EndDateReport = endDateReport;
            }

            return ResponseResult.PostResult(
                reportData,
                status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<byte[]> GenerateReportAsync(FilterReservationReportDto filter, CancellationToken cancellationToken = default)
        {
            string fileDirPath = Assembly.GetExecutingAssembly().Location.Replace("SonoBooking.Application.dll", string.Empty);
            string rdclFilePath = string.Format(@"{0}ReportsFiles\{1}.rdlc", fileDirPath, filter.ReportName);

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("utf-8");

            LocalReport report = new()
            {
                ReportPath = rdclFilePath
            };

            IFinalResult reportResult = null;

            if (filter.ReportName == "ReservationDetailsReport")
            {
                reportResult = await GetAllReportAsync(filter, cancellationToken);
                List<ReservationReportDto> reportData = (reportResult.Data as IEnumerable<ReservationReportDto>)?.ToList()
                    ?? throw new InvalidOperationException("No data found for the report.");

                foreach (ReservationReportDto row in reportData)
                    row.User = _user.Name;

                report.DataSources.Add(new ReportDataSource() { Name = "ReservationDetailsReport", Value = reportData });
            }

            if (reportResult == null || reportResult.Data == null)
                throw new InvalidOperationException("Failed to retrieve report data.");

            byte[] renderedBytes = [];
            try
            {
                renderedBytes = report.Render(filter.ReportType);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new InvalidOperationException("Error rendering report: " + ex.Message, ex);
            }

            return renderedBytes;
        }

        public override async Task<IFinalResult> AddAsync(AddReservationDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                IFinalResult totalResult = await ApplyTotalAmountFromRequestAsync(model, cancellationToken);
                if (totalResult != null)
                    return totalResult;

                IFinalResult validation = ValidateReservationDto(model);
                if (validation != null)
                    return validation;

                Reservation entity = Mapper.Map<Reservation>(model);

                SetEntityCreatedBaseProperties(entity);

                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.AddError);

                Reservation reservationWithRequest = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id == entity.Id,
                    include: src => src.Include(r => r.Request),
                    disableTracking: true,
                    cancellationToken: cancellationToken) ?? entity;

                await housingNotificationService.NotifyReceptionOnNewReservationAsync(
                    reservationWithRequest,
                    cancellationToken);

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

        public override async Task<IFinalResult> UpdateAsync(AddReservationDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                IFinalResult totalResult = await ApplyTotalAmountFromRequestAsync(model, cancellationToken);
                if (totalResult != null)
                    return totalResult;

                IFinalResult validation = ValidateReservationDto(model);
                if (validation != null)
                    return validation;

                Reservation entityToUpdate = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id.Equals(model.Id),
                    include: src => src
                        .Include(r => r.Payment)
                        .Include(r => r.Request),
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToUpdate == null)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.NotFound, exception: null,
                        message: MessagesConstants.NotFound);

                ReservationStatus previousStatus = entityToUpdate.Status;
                Reservation entity = Mapper.Map(model, entityToUpdate);

                entity.RequestId = entityToUpdate.RequestId;

                if (IsSuperAdmin())
                    entity.IsDeleted = false;

                SetEntityModifiedBaseProperties(entity);

                UnitOfWork.Repository.Update(entityToUpdate, entity);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.UpdateError);

                if (previousStatus != entityToUpdate.Status)
                    await statusEmailNotifier.TrySendStatusChangeEmailAsync(
                        entityToUpdate, previousStatus, cancellationToken);

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
                Reservation entityToDelete = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id.Equals(id),
                    include: src => src.Include(r => r.Payment),
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToDelete == null)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.NotFound, exception: null,
                        message: MessagesConstants.NotFound);

                IEnumerable<Extension> extensions = await UnitOfWork.GetRepository<Extension>().FindAsync(
                    x => x.ReservationId == entityToDelete.Id,
                    cancellationToken: cancellationToken);

                UnitOfWork.GetRepository<Extension>().RemoveRange(extensions, cancellationToken);
                if (entityToDelete.Payment != null)
                    UnitOfWork.GetRepository<Payment>().Remove(entityToDelete.Payment);
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

        private static Expression<Func<Reservation, bool>> PredicateBuilderReportFunction(FilterReservationReportDto filter)
        {
            var predicate = PredicateBuilder.New<Reservation>(x => x.IsDeleted != true);

            if (filter.StartDate != default)
                predicate = predicate.And(e => e.StartDate >= filter.StartDate);

            if (filter.EndDate != default)
                predicate = predicate.And(e => e.EndDate <= filter.EndDate);

            if (filter.ReservationStatus.HasValue)
                predicate = predicate.And(e => e.Status == filter.ReservationStatus.Value);

            return predicate;
        }

        private string GetUserIdFromHeader() =>
            HttpContextAccessor?.HttpContext?.Request.Headers["UserId"].FirstOrDefault()?.Trim();

        /// <summary>
        /// Sum of linked bed/room/apartment nightly prices × request nights.
        /// Client-supplied <see cref="AddReservationDto.TotalAmount"/> is ignored.
        /// </summary>
        private async Task<IFinalResult> ApplyTotalAmountFromRequestAsync(
            AddReservationDto model,
            CancellationToken cancellationToken)
        {
            string requestId = model.RequestId?.Trim();
            if (string.IsNullOrWhiteSpace(requestId))
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "RequestId is required.");

            Request request = await UnitOfWork.GetRepository<Request>().FirstOrDefaultAsync(
                x => x.Id == requestId,
                include: src => src
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Bed)
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Room)
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Apartment),
                disableTracking: true,
                cancellationToken: cancellationToken);

            if (request == null)
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "Request not found.");

            model.TotalAmount = CalculateTotalAmountForRequest(request);
            return null;
        }

        private static decimal CalculateTotalAmountForRequest(Request request)
        {
            int nights = request.EndDate.DayNumber - request.StartDate.DayNumber;
            if (nights <= 0)
                nights = 1;

            decimal nightlySum = request.RequestUnits
                .Where(u => !u.IsDeleted)
                .Sum(ResolveRequestUnitNightlyPrice);

            return nightlySum * nights;
        }

        private static decimal ResolveRequestUnitNightlyPrice(RequestUnit unit)
        {
            if (!string.IsNullOrWhiteSpace(unit.BedId) && unit.Bed != null)
                return unit.Bed.Price;

            if (!string.IsNullOrWhiteSpace(unit.RoomId) && unit.Room != null)
                return unit.Room.Price;

            if (!string.IsNullOrWhiteSpace(unit.ApartmentId) && unit.Apartment != null)
                return unit.Apartment.Price;

            return 0;
        }

        private IFinalResult ValidateReservationDto(AddReservationDto model)
        {
            if (model.EndDate < model.StartDate)
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "EndDate must be on or after StartDate.");

            if (string.IsNullOrWhiteSpace(model.RequestId))
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "RequestId is required.");

            if (model.TotalAmount < 0)
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "TotalAmount must be zero or greater.");

            return null;
        }
    }
}
