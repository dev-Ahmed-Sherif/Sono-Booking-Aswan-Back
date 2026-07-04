using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using SonoBooking.Application.Services.Base;
using SonoBooking.Application.Services.Email;
using SonoBooking.Application.Services.Housing.Notifications;
using SonoBooking.Application.Services.LookUp.Attachments;
using SonoBooking.Common.Constants;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.Base;
using SonoBooking.Common.DTO.Housing.Request;
using SonoBooking.Common.DTO.Housing.Request.Parameters;
using SonoBooking.Common.DTO.Housing.RequestAttach;
using SonoBooking.Common.DTO.Housing.RequestParticipant;
using SonoBooking.Common.DTO.Housing.RequestUnit;
using SonoBooking.Common.DTO.Lookup.Attachment;
using SonoBooking.Common.DTO.Reports.Requests;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.Housing;
using SonoBooking.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.Housing.Requests
{
    public class RequestService(
        IServiceBaseParameter<Request> businessBaseParameter,
        IAttachmentService attachmentService,
        IEmailService emailService,
        HousingNotificationService housingNotificationService) : BaseService<Request, AddRequestDto, EditRequestDto, RequestDto, string, string>(businessBaseParameter), IRequestService
    {
        public override async Task<IFinalResult> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            Request entity = await UnitOfWork.Repository.FirstOrDefaultAsync(
                x => x.Id.Equals(id),
                include: src => src
                    .Include(r => r.RequestType)
                    .Include(r => r.User)
                    .Include(r => r.RequestTo)
                    .Include(r => r.RequestAttaches).ThenInclude(a => a.Attachment),
                disableTracking: true,
                cancellationToken: cancellationToken);

            if (entity == null)
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.NotFound, exception: null,
                    message: MessagesConstants.NotFound);

            RequestDto mapped = Mapper.Map<Request, RequestDto>(entity);
            mapped.RequestAttaches = [.. entity.RequestAttaches.Select(a => new RequestAttachDto
            {
                Id = a.Id,
                RequestId = a.RequestId,
                AttachmentId = a.AttachmentId,
                FileName = a.Attachment?.FileName,
                Extension = a.Attachment?.Extension,
                Url = a.Attachment?.Url,
                IsPrimary = a.IsPrimary
            })];

            return ResponseResult.PostResult(result: mapped, status: HttpStatusCode.OK, exception: null,
                message: MessagesConstants.Success);
        }

        public override async Task<IFinalResult> GetAllAsync(
            bool disableTracking = false,
            Expression<Func<Request, bool>> predicate = null,
            CancellationToken cancellationToken = default)
        {
            string userId = GetUserIdFromHeader();
            string leaderScopeId = GetLeaderScopeId();

            IEnumerable<Request> query;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                string trimmedUserId = userId.Trim();
                query = await UnitOfWork.Repository.FindAsync(
                    x => x.UserId == trimmedUserId,
                    include: src => src.Include(r => r.RequestType).Include(r => r.RequestTo),
                    disableTracking: disableTracking,
                    cancellationToken: cancellationToken);
            }
            else if (!string.IsNullOrWhiteSpace(leaderScopeId))
            {
                query = await UnitOfWork.Repository.FindAsync(
                    x => x.RequestToId == leaderScopeId,
                    include: src => src.Include(r => r.RequestType).Include(r => r.RequestTo),
                    disableTracking: disableTracking,
                    cancellationToken: cancellationToken);
            }
            else if (predicate != null)
            {
                query = await UnitOfWork.Repository.FindAsync(
                    predicate,
                    include: src => src.Include(r => r.RequestType).Include(r => r.RequestTo),
                    disableTracking: disableTracking,
                    cancellationToken: cancellationToken);
            }
            else
            {
                query = await UnitOfWork.Repository.GetAllAsync(
                    include: src => src.Include(r => r.RequestType).Include(r => r.RequestTo),
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
            string leaderScopeId = GetLeaderScopeId();

            (int Count, IEnumerable<Request> Result) = await UnitOfWork.Repository.FindPagedAsync(
                predicate: x => x.IsDeleted == requestFilter.IsDeleted
                    && (string.IsNullOrWhiteSpace(leaderScopeId) || x.RequestToId == leaderScopeId),
                pageNumber: offset,
                pageSize: limit,
                filter.OrderByValue,
                include: src => src.Include(r => r.RequestType).Include(r => r.RequestTo),
                cancellationToken: cancellationToken);

            IEnumerable<RequestDto> data = Mapper.Map<IEnumerable<Request>, IEnumerable<RequestDto>>(Result ?? []);
            return new PagingResult(pageNumber, limit, Count, data, status: HttpStatusCode.OK, MessagesConstants.Success);
        }
        public async Task<IFinalResult> GetAllReportAsync(FilterRequestReportDto filter, CancellationToken cancellationToken = default)
        {
            IEnumerable<Request> query = await UnitOfWork.Repository.FindAsync(
                predicate: PredicateBuilderReportFunction(filter),
                include: src => src.Include(r => r.Reservation).ThenInclude(re => re.Payment),
                cancellationToken: cancellationToken);

            List<Request> requests = [.. query];

            int totalCount = requests.Count;
            int acceptedCount = requests.Count(r => r.Status == Status.Approved);
            int rejectedCount = requests.Count(r => r.Status == Status.Rejected);
            // Approved requests with an active reservation (not canceled / no-show).
            int confirmedReservationCount = requests.Count(r =>
                r.Status == Status.Approved &&
                r.Reservation != null &&
                r.Reservation.Status != ReservationStatus.Canceled &&
                r.Reservation.Status != ReservationStatus.NoShow);

            float totalRevenue = (float)requests
                .Where(r => r.Reservation != null && r.Status == Status.Approved)
                .Sum(r => r.Reservation!.Payment.Amount);

            RequestReportDto reportData = new()
            {
                TotalRequestCount = totalCount,
                TotalAcceptedRequestCount = acceptedCount,
                TotalRejectedRequestCount = rejectedCount,
                TotalAcceptedReservationCount = confirmedReservationCount,
                TotalRevenue = totalRevenue,
                StartDateReport = filter.StartDate.ToString("dd/MM/yyyy"),
                EndDateReport = filter.EndDate.ToString("dd/MM/yyyy")
            };

            return ResponseResult.PostResult(
                new[] { reportData },
                status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<IFinalResult> GetRequestDetailsReportAsync(FilterRequestReportDto filter, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filter.RequestId))
            {
                return ResponseResult.PostResult(
                    result: null,
                    status: HttpStatusCode.BadRequest,
                    exception: null,
                    message: "RequestId is required for RequestDetailsReport.");
            }

            (RequestDetailsReportDto details, List<RequestDetailsCompanionReportDto> companions) =
                await BuildRequestDetailsReportDataAsync(filter.RequestId.Trim(), cancellationToken);

            return ResponseResult.PostResult(
                new
                {
                    Details = details,
                    Companions = companions
                },
                status: HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }

        public async Task<byte[]> GenerateReportAsync(FilterRequestReportDto filter, CancellationToken cancellationToken = default)
        {
            // get report file
            string fileDirPath = Assembly.GetExecutingAssembly().Location.Replace("SonoBooking.Application.dll", string.Empty);
            Console.WriteLine(string.Format(@"{0}ReportsFiles\{1}.rdlc", fileDirPath, filter.ReportName));
            string rdclFilePath = string.Format(@"{0}ReportsFiles\{1}.rdlc", fileDirPath, filter.ReportName);

            // file encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("utf-8");

            LocalReport report = new()
            {
                ReportPath = rdclFilePath
                //ReportPath = fileDirPath
            };

            // prepare data for report
            IFinalResult request = null; // Initialize Org to avoid CS0165 error

            if (filter.ReportName == "RequestReport")
            {
                request = await GetAllReportAsync(filter, cancellationToken);
                List<RequestReportDto> reportData = (request.Data as IEnumerable<RequestReportDto>)?.ToList()
                    ?? throw new InvalidOperationException("No data found for the report.");

                foreach (RequestReportDto row in reportData)
                    row.User = _user.Name;

                report.DataSources.Add(new ReportDataSource() { Name = "RequestReport", Value = reportData });
                report.DataSources.Add(new ReportDataSource()
                {
                    Name = "RequestReportChart",
                    Value = BuildRequestReportChartData(reportData[0])
                });
            }
            else if (filter.ReportName == "RequestDetailsReport")
            {
                if (string.IsNullOrWhiteSpace(filter.RequestId))
                    throw new InvalidOperationException("RequestId is required for RequestDetailsReport.");

                (RequestDetailsReportDto details, List<RequestDetailsCompanionReportDto> companions) =
                    await BuildRequestDetailsReportDataAsync(filter.RequestId.Trim(), cancellationToken);

                details.User = _user.Name;

                report.DataSources.Add(new ReportDataSource() { Name = "RequestDetails", Value = new[] { details } });
                report.DataSources.Add(new ReportDataSource() { Name = "RequestDetailsCompanion", Value = companions });

                request = ResponseResult.PostResult(details, HttpStatusCode.OK, null, HttpStatusCode.OK.ToString());
            }

            if (request == null || request.Data == null)
            {
                throw new InvalidOperationException("Failed to retrieve report data.");
            }

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

            //byte[] renderedBytes = report.Render("");

            return renderedBytes;
        }

        public override async Task<IFinalResult> AddAsync(AddRequestDto model, CancellationToken cancellationToken = default)
        {
            try
            {
                IFinalResult? leaderValidation = await ValidateRequestToLeaderAsync(model.RequestToId, cancellationToken);
                if (leaderValidation != null)
                    return leaderValidation;

                Request entity = Mapper.Map<Request>(model);
                entity.RequestNumber = await GenerateRequestNumberAsync(cancellationToken);
                entity.RequestDate = DateTime.UtcNow;
                entity.Status = Status.Pending;
                entity.UserId = !string.IsNullOrWhiteSpace(_user.Id) ? _user.Id : entity.CreatedById;

                await AddRequestAttachmentsAsync(entity, model.Images, cancellationToken);

                SetEntityCreatedBaseProperties(entity);

                await UnitOfWork.Repository.AddAsync(entity, cancellationToken);
                await SyncRequestUnitsAsync(entity.Id, model.RequestUnits, [], cancellationToken);
                await SyncRequestParticipantsAsync(entity.Id, model.RequestCompanions, [], cancellationToken);

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows <= 0)
                    return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.AddError);

                await housingNotificationService.NotifyLeadersOnNewRequestAsync(entity, cancellationToken);

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
                IFinalResult? leaderValidation = await ValidateRequestToLeaderAsync(model.RequestToId, cancellationToken);
                if (leaderValidation != null)
                    return leaderValidation;

                Request entityToUpdate = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id.Equals(model.Id),
                    include: src => src
                        .Include(r => r.RequestUnits)
                        .Include(r => r.RequestParticipants)
                        .Include(r => r.RequestAttaches),
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToUpdate == null)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.NotFound, exception: null,
                        message: MessagesConstants.NotFound);

                Request entity = Mapper.Map(model, entityToUpdate);

                entity.RequestNumber = entityToUpdate.RequestNumber;
                entity.RequestDate = entityToUpdate.RequestDate;
                entity.UserId = entityToUpdate.UserId;

                Status previousStatus = entityToUpdate.Status;
                entity.Status = model.Status ?? entityToUpdate.Status;
                bool isNewlyApproved = previousStatus != Status.Approved && entity.Status == Status.Approved;
                bool isNewlyRejected = previousStatus != Status.Rejected && entity.Status == Status.Rejected;

                if (isNewlyApproved && previousStatus != Status.Pending)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                        message: "Only pending requests can be approved.");

                if (isNewlyApproved)
                {
                    entity.ApprovedById = !string.IsNullOrWhiteSpace(model.ApprovedById)
                        ? model.ApprovedById
                        : (!string.IsNullOrWhiteSpace(_user.Id) ? _user.Id : null);
                    entity.ApprovedAt = model.ApprovedAt ?? DateTime.UtcNow;
                }

                if (isNewlyRejected && !string.IsNullOrWhiteSpace(model.RejectionReason))
                    entity.RejectionReason = model.RejectionReason.Trim();

                if (IsSuperAdmin())
                    entity.IsDeleted = false;

                await AddRequestAttachmentsAsync(entity, model.Images, cancellationToken);

                if (model.OldImages != null)
                {
                    HashSet<string> keptAttachIds = model.OldImages
                        .Where(o => !string.IsNullOrWhiteSpace(o.Id))
                        .Select(o => o.Id.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);

                    List<RequestAttach> attachesToRemove = entity.RequestAttaches
                        .Where(ra => !keptAttachIds.Contains(ra.Id))
                        .ToList();

                    if (attachesToRemove.Count > 0)
                    {
                        List<string> attachIds = [.. attachesToRemove
                            .Select(a => a.AttachmentId)
                            .Where(id => !string.IsNullOrWhiteSpace(id))];

                        foreach (RequestAttach removedAttach in attachesToRemove)
                            entity.RequestAttaches.Remove(removedAttach);

                        await UnitOfWork.SaveChangesAsync(cancellationToken);

                        if (attachIds.Count > 0)
                            await attachmentService.DeleteRangeAsync(attachIds, cancellationToken);
                    }

                    foreach (AddRequestAttachDto oldImage in model.OldImages)
                    {
                        RequestAttach existingAttach = entity.RequestAttaches
                            .FirstOrDefault(ra => ra.Id == oldImage.Id);

                        if (existingAttach != null)
                            existingAttach.IsPrimary = oldImage.IsPrimary;
                    }
                }

                SetEntityModifiedBaseProperties(entity);

                UnitOfWork.Repository.Update(entityToUpdate, entity);

                await SyncRequestUnitsAsync(entity.Id, model.RequestUnits, entityToUpdate.RequestUnits.ToList(), cancellationToken);
                await SyncRequestParticipantsAsync(entity.Id, model.RequestCompanions, entityToUpdate.RequestParticipants.ToList(), cancellationToken);

                if (isNewlyApproved && entity.RequestAllocationType == AllocationType.Flexible)
                {
                    Request pendingWithUnits = await UnitOfWork.Repository.FirstOrDefaultAsync(
                        x => x.Id == entity.Id,
                        include: src => src
                            .Include(r => r.RequestUnits).ThenInclude(u => u.Bed).ThenInclude(b => b.Room)
                            .Include(r => r.RequestUnits).ThenInclude(u => u.Room),
                        disableTracking: true,
                        cancellationToken: cancellationToken);

                    IFinalResult overlapValidation = await ValidateFlexibleApprovalNoOverlapAsync(
                        pendingWithUnits,
                        cancellationToken);
                    if (overlapValidation != null)
                        return overlapValidation;
                }

                List<Request> autoRejectedRequests = [];
                if (isNewlyApproved)
                {
                    Request approvedRequest = await UnitOfWork.Repository.FirstOrDefaultAsync(
                        x => x.Id == entity.Id,
                        include: src => src
                            .Include(r => r.RequestUnits).ThenInclude(u => u.Bed).ThenInclude(b => b.Room)
                            .Include(r => r.RequestUnits).ThenInclude(u => u.Room),
                        disableTracking: false,
                        cancellationToken: cancellationToken);

                    if (approvedRequest != null)
                        autoRejectedRequests = await RejectConflictingFixedRequestsAsync(approvedRequest, cancellationToken);
                }

                if (isNewlyApproved && entity.RequestCatagory == RequestCatagory.Extension)
                {
                    await SyncRootReservationCheckoutOnExtensionApprovalAsync(
                        entity,
                        cancellationToken);
                }

                int affectedRows = await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (affectedRows < 0)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.BadRequest, exception: null,
                        message: MessagesConstants.UpdateError);

                if (isNewlyApproved || isNewlyRejected)
                {
                    await TrySendRequestStatusEmailAsync(entity, cancellationToken);
                    await housingNotificationService.NotifyOwnerOnRequestDecisionAsync(
                        entity,
                        _user?.Name,
                        cancellationToken);
                }

                foreach (Request rejectedRequest in autoRejectedRequests)
                {
                    await TrySendRequestStatusEmailAsync(rejectedRequest, cancellationToken);
                    await housingNotificationService.NotifyOwnerOnRequestDecisionAsync(
                        rejectedRequest,
                        _user?.Name,
                        cancellationToken);
                }

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
                        .Include(r => r.RequestAttaches)
                        .Include(r => r.Approval)
                        .Include(r => r.Reservation).ThenInclude(res => res.Payment),
                    disableTracking: false,
                    cancellationToken: cancellationToken);

                if (entityToDelete == null)
                    return ResponseResult.PostResult(result: false, status: HttpStatusCode.NotFound, exception: null,
                        message: MessagesConstants.NotFound);

                List<string> attachIds = [.. entityToDelete.RequestAttaches
                    .Select(a => a.AttachmentId)
                    .Where(id => !string.IsNullOrWhiteSpace(id))];

                UnitOfWork.GetRepository<RequestAttach>().RemoveRange(entityToDelete.RequestAttaches, cancellationToken);
                entityToDelete.RequestAttaches.Clear();
                await UnitOfWork.SaveChangesAsync(cancellationToken);

                if (attachIds.Count > 0)
                    await attachmentService.DeleteRangeAsync(attachIds, cancellationToken);

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

        private async Task<(RequestDetailsReportDto Details, List<RequestDetailsCompanionReportDto> Companions)> BuildRequestDetailsReportDataAsync(
            string requestId,
            CancellationToken cancellationToken)
        {
            Request entity = await UnitOfWork.Repository.FirstOrDefaultAsync(
                x => x.Id.Equals(requestId),
                include: src => src
                    .Include(r => r.User)
                    .Include(r => r.RequestTo)
                    .Include(r => r.RequestType)
                    .Include(r => r.RequestAttaches).ThenInclude(a => a.Attachment)
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Apartment).ThenInclude(a => a.Governorate)
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Room).ThenInclude(room => room.Apartment).ThenInclude(a => a.Governorate)
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Bed).ThenInclude(b => b.Room).ThenInclude(room => room.Apartment).ThenInclude(a => a.Governorate)
                    .Include(r => r.RequestParticipants).ThenInclude(p => p.Companion).ThenInclude(c => c.Relationship),
                disableTracking: true,
                cancellationToken: cancellationToken);

            if (entity == null)
                throw new InvalidOperationException("Request not found.");

            return (MapRequestDetailsReport(entity), MapRequestDetailsCompanions(entity));
        }

        private static RequestDetailsReportDto MapRequestDetailsReport(Request entity)
        {
            DateTime requestDate = entity.RequestDate;
            int nights = entity.EndDate.DayNumber - entity.StartDate.DayNumber;
            if (nights <= 0)
                nights = 1;

            string typeCode = ResolveRequestTypeCode(entity);
            string unitGovernorate = entity.RequestUnits?
                .Where(u => !u.IsDeleted)
                .Select(ResolveRequestUnitGovernorateName)
                .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name))
                ?? "أسوان";

            IEnumerable<RequestUnit> activeUnits = entity.RequestUnits?.Where(u => !u.IsDeleted) ?? [];
            int apartments = activeUnits.Count(u => !string.IsNullOrWhiteSpace(u.ApartmentId));
            int rooms = activeUnits.Count(u => !string.IsNullOrWhiteSpace(u.RoomId));
            int beds = activeUnits.Count(u => !string.IsNullOrWhiteSpace(u.BedId));

            bool isWorkMission = typeCode.Equals("mission", StringComparison.OrdinalIgnoreCase);
            bool isApproved = entity.Status == Status.Approved;
            bool isRejected = entity.Status == Status.Rejected;
            string statusUpdatedAt = (isApproved || isRejected) ? entity.ModifiedAt.ToString("yyyy/MM/dd") : string.Empty;
            byte[]? leaderSignature = entity.RequestTo?.FileContent is { Length: > 0 } content ? content : null;
            bool showLeaderSignature = (isApproved || isRejected) && leaderSignature != null;

            return new RequestDetailsReportDto
            {
                RequestDate = requestDate.ToString("dd/MM/yyyy"),
                RequestDateDay = requestDate.Day.ToString("00"),
                RequestDateMonth = requestDate.Month.ToString("00"),
                RequestDateYear = requestDate.Year.ToString(),
                LeaderFullName = entity.RequestTo?.FullName ?? string.Empty,
                LeaderPosition = entity.RequestTo?.Position ?? "محافظ أسوان",
                ApplicantName = entity.User?.FullName ?? string.Empty,
                NationalId = entity.User?.DocumentNumber ?? string.Empty,
                JobTitle = entity.User?.JobTitle ?? string.Empty,
                Employer = entity.User?.Organization ?? string.Empty,
                UnitGovernorate = unitGovernorate,
                DestinationGovernorate = unitGovernorate,
                StartDate = entity.StartDate.ToString("dd/MM/yyyy"),
                EndDate = entity.EndDate.ToString("dd/MM/yyyy"),
                Nights = nights.ToString(),
                Apartments = apartments.ToString(),
                Rooms = rooms.ToString(),
                Beds = beds.ToString(),
                IsWorkMission = PurposeCheckboxMark(isWorkMission),
                IsMedical = PurposeCheckboxMark(typeCode.Equals("medical", StringComparison.OrdinalIgnoreCase)),
                IsSpecial = PurposeCheckboxMark(typeCode.Equals("personal", StringComparison.OrdinalIgnoreCase)),
                Phone = entity.User?.PhoneNumber ?? string.Empty,
                Attachments = BuildRequestDetailsAttachments(isWorkMission),
                RejectionReason = isRejected
                    ? (string.IsNullOrWhiteSpace(entity.RejectionReason) ? "لم يتم تحديد سبب" : entity.RejectionReason.Trim())
                    : string.Empty,
                ShowApprovedStamp = PurposeCheckboxMark(isApproved),
                ShowRejectedStamp = PurposeCheckboxMark(isRejected),
                StatusUpdatedAt = statusUpdatedAt,
                StatusUpdatedAtImage = GenerateRotatedStatusDateImage(statusUpdatedAt, isRejected),
                LeaderSignatureImage = leaderSignature,
                LeaderSignatureMimeType = leaderSignature == null ? string.Empty : ResolveImageMimeType(leaderSignature),
                ShowLeaderSignature = PurposeCheckboxMark(showLeaderSignature)
            };
        }

        private static string ResolveImageMimeType(byte[] content)
        {
            if (content.Length >= 2 && content[0] == 0xFF && content[1] == 0xD8)
                return "image/jpeg";

            return "image/png";
        }

        private static byte[]? GenerateRotatedStatusDateImage(string dateText, bool isRejected)
        {
            if (string.IsNullOrWhiteSpace(dateText))
                return null;

            const float stampRotationDegrees = -22f;
            int imageWidth = isRejected ? 368 : 351;
            int imageHeight = isRejected ? 346 : 325;
            float anchorX = isRejected ? 208f : 183f;
            float anchorY = isRejected ? 170f : 167f;

            using Bitmap bitmap = new(imageWidth, imageHeight, PixelFormat.Format32bppArgb);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            using Font font = new("Arial", 22f, FontStyle.Bold, GraphicsUnit.Point);
            using Brush brush = new SolidBrush(Color.FromArgb(0x2E, 0x75, 0xB6));
            using StringFormat format = new(StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            graphics.TranslateTransform(anchorX, anchorY);
            graphics.RotateTransform(stampRotationDegrees);
            graphics.DrawString(dateText, font, brush, 0f, 0f, format);

            using MemoryStream memoryStream = new();
            bitmap.Save(memoryStream, ImageFormat.Png);
            return memoryStream.ToArray();
        }

        private static List<RequestDetailsCompanionReportDto> MapRequestDetailsCompanions(Request entity)
        {
            List<RequestDetailsCompanionReportDto> companions = [];
            int rowNumber = 1;
            DateOnly referenceDate = entity.StartDate;

            foreach (RequestParticipant participant in entity.RequestParticipants.Where(p => !p.IsDeleted))
            {
                Companion? companion = participant.Companion;
                if (companion == null)
                    continue;

                int age = referenceDate.Year - companion.BirthDate.Year;
                if (companion.BirthDate > referenceDate.AddYears(-age))
                    age--;

                companions.Add(new RequestDetailsCompanionReportDto
                {
                    RowNumber = rowNumber.ToString(),
                    Name = companion.FullName,
                    Relationship = companion.Relationship?.NameAr ?? string.Empty,
                    Age = age.ToString()
                });
                rowNumber++;
            }

            if (companions.Count > 2)
                companions = companions.Take(2).ToList();

            return companions;
        }

        private static string? ResolveRequestUnitGovernorateName(RequestUnit unit)
        {
            string? fromApartment = unit.Apartment?.Governorate?.NameAr;
            if (!string.IsNullOrWhiteSpace(fromApartment))
                return fromApartment.Trim();

            string? fromRoom = unit.Room?.Apartment?.Governorate?.NameAr;
            if (!string.IsNullOrWhiteSpace(fromRoom))
                return fromRoom.Trim();

            string? fromBed = unit.Bed?.Room?.Apartment?.Governorate?.NameAr;
            if (!string.IsNullOrWhiteSpace(fromBed))
                return fromBed.Trim();

            return null;
        }

        private static string ResolveRequestTypeCode(Request entity)
        {
            string? code = entity.RequestType?.Code?.Trim();
            if (!string.IsNullOrWhiteSpace(code))
                return code;

            string nameAr = entity.RequestType?.NameAr?.Trim() ?? string.Empty;
            if (nameAr.Contains("مأمور", StringComparison.OrdinalIgnoreCase))
                return "mission";
            if (nameAr.Contains("طبي", StringComparison.OrdinalIgnoreCase) || nameAr.Contains("علاج", StringComparison.OrdinalIgnoreCase))
                return "medical";
            if (nameAr.Contains("شخص", StringComparison.OrdinalIgnoreCase) || nameAr.Contains("خاص", StringComparison.OrdinalIgnoreCase))
                return "personal";

            return string.Empty;
        }

        private static string PurposeCheckboxMark(bool selected) => selected ? "1" : string.Empty;

        private static string BuildRequestDetailsAttachments(bool isWorkMission)
        {
            List<string> lines =
            [
                FormatAttachmentReportLine("صورة بطاقة الرقم القومي (سارية).")
            ];

            if (isWorkMission)
                lines.Add(FormatAttachmentReportLine("خطاب جهة العمل في حالة القيام بمأمورية عمل."));

            return string.Join("\r\n", lines);
        }

        private static string FormatAttachmentReportLine(string text) =>
            $"\u202B• {text.Trim()}\u202C";

        private static List<RequestReportChartItemDto> BuildRequestReportChartData(RequestReportDto data)
        {
            int total = data.TotalRequestCount;

            return
            [
                new() { Category = "عدد الطلبات", Value = total, Percentage = 0 },
                new()
                {
                    Category = "الموافق عليها",
                    Value = data.TotalAcceptedRequestCount,
                    Percentage = CalculateReportPercentage(data.TotalAcceptedRequestCount, total)
                },
                new()
                {
                    Category = "المرفوضة",
                    Value = data.TotalRejectedRequestCount,
                    Percentage = CalculateReportPercentage(data.TotalRejectedRequestCount, total)
                },
                new()
                {
                    Category = "الحجوزات المؤكدة",
                    Value = data.TotalAcceptedReservationCount,
                    Percentage = CalculateReportPercentage(data.TotalAcceptedReservationCount, total)
                },
            ];
        }

        private static float CalculateReportPercentage(int part, int total) =>
            total == 0 ? 0f : (float)part / total * 100f;

        static Expression<Func<Request, bool>> PredicateBuilderReportFunction(FilterRequestReportDto filter)
        {
            var predicate = PredicateBuilder.New<Request>(x => x.IsDeleted != true && x.Status != Status.Canceled);

            if (filter.StartDate != default)
            {
                predicate = predicate.And(e => e.StartDate >= filter.StartDate);
            }
            if (filter.EndDate != default)
            {
                predicate = predicate.And(e => e.EndDate <= filter.EndDate);
            }

            return predicate;
        }

        private async Task<string> GenerateRequestNumberAsync(CancellationToken cancellationToken)
        {
            int year = DateTime.UtcNow.Year;
            string prefix = $"REQ-{year}-";

            IEnumerable<string> requestNumbers = await UnitOfWork.Repository.FindSelectAsync(
                r => r.RequestNumber,
                r => r.RequestDate.Year == year,
                disableTracking: true,
                cancellationToken: cancellationToken);

            int nextSequence = requestNumbers
                .Where(number => !string.IsNullOrWhiteSpace(number) &&
                                 number.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .Select(ParseRequestSequence)
                .DefaultIfEmpty(0)
                .Max() + 1;

            return $"{prefix}{nextSequence:D4}";
        }

        private static int ParseRequestSequence(string requestNumber)
        {
            int lastDash = requestNumber.LastIndexOf('-');
            if (lastDash < 0 || lastDash >= requestNumber.Length - 1)
                return 0;

            return int.TryParse(requestNumber.AsSpan(lastDash + 1), out int sequence) ? sequence : 0;
        }

        private string GetUserIdFromHeader() =>
            HttpContextAccessor?.HttpContext?.Request.Headers["UserId"].FirstOrDefault()?.Trim();

        private string GetLeaderScopeId() =>
            IsSuperAdmin() || string.IsNullOrWhiteSpace(_user.LeaderId)
                ? string.Empty
                : _user.LeaderId.Trim();

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

        private async Task SyncRootReservationCheckoutOnExtensionApprovalAsync(
            Request extensionRequest,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(extensionRequest.PreviousRequestId))
                return;

            string rootRequestId = extensionRequest.PreviousRequestId.Trim();
            HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
            while (true)
            {
                if (!visited.Add(rootRequestId))
                    break;

                Request? chain = await UnitOfWork.Repository.FirstOrDefaultAsync(
                    x => x.Id == rootRequestId,
                    disableTracking: true,
                    cancellationToken: cancellationToken);
                if (chain == null)
                    return;

                if (string.IsNullOrWhiteSpace(chain.PreviousRequestId))
                    break;

                rootRequestId = chain.PreviousRequestId.Trim();
            }

            Reservation? reservation = await UnitOfWork.GetRepository<Reservation>().FirstOrDefaultAsync(
                r => !r.IsDeleted && r.RequestId == rootRequestId,
                disableTracking: false,
                cancellationToken: cancellationToken);

            if (reservation == null)
                return;

            if (extensionRequest.EndDate > reservation.EndDate)
                reservation.EndDate = extensionRequest.EndDate;

            if (reservation.ActualCheckOutDate != null
                && extensionRequest.EndDate >
                    DateOnly.FromDateTime(reservation.ActualCheckOutDate.Value.Date))
            {
                reservation.ActualCheckOutDate =
                    extensionRequest.EndDate.ToDateTime(new TimeOnly(12, 0, 0));
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

        private async Task<List<RequestUnit>> ResolveRequestUnitsForOverlapAsync(
            Request request,
            CancellationToken cancellationToken)
        {
            List<RequestUnit> units = request.RequestUnits?
                .Where(u => !u.IsDeleted)
                .ToList() ?? [];

            if (units.Count > 0)
                return units;

            if (request.RequestCatagory != RequestCatagory.Extension
                || string.IsNullOrWhiteSpace(request.PreviousRequestId))
            {
                return units;
            }

            Request previous = await UnitOfWork.Repository.FirstOrDefaultAsync(
                x => x.Id == request.PreviousRequestId,
                include: src => src
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Bed).ThenInclude(b => b.Room)
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Room),
                disableTracking: true,
                cancellationToken: cancellationToken);

            return previous?.RequestUnits
                .Where(u => !u.IsDeleted)
                .ToList() ?? units;
        }

        private async Task<IFinalResult> ValidateFlexibleApprovalNoOverlapAsync(
            Request pending,
            CancellationToken cancellationToken)
        {
            if (pending == null || pending.RequestAllocationType != AllocationType.Flexible)
                return null;

            List<RequestUnit> pendingUnits = await ResolveRequestUnitsForOverlapAsync(
                pending,
                cancellationToken);
            if (pendingUnits.Count == 0)
                return null;

            IEnumerable<Request> approvedRequests = await UnitOfWork.Repository.FindAsync(
                predicate: r => !r.IsDeleted
                    && r.Id != pending.Id
                    && r.Status == Status.Approved,
                include: src => src
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Bed).ThenInclude(b => b.Room)
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Room),
                disableTracking: true,
                cancellationToken: cancellationToken);

            foreach (Request approved in approvedRequests)
            {
                if (!RequestDateRangesOverlap(pending, approved))
                    continue;

                List<RequestUnit> approvedUnits = await ResolveRequestUnitsForOverlapAsync(
                    approved,
                    cancellationToken);
                if (approvedUnits.Count == 0)
                    continue;

                List<RequestUnit> allUnits = [.. pendingUnits, .. approvedUnits];
                IReadOnlyDictionary<string, string> bedRoomIds =
                    await GetBedRoomIdsAsync(allUnits, cancellationToken);
                IReadOnlyDictionary<string, string> roomApartmentIds =
                    await GetRoomApartmentIdsAsync(allUnits, bedRoomIds, cancellationToken);

                if (RequestUnitsOverlap(approvedUnits, pendingUnits, bedRoomIds, roomApartmentIds))
                {
                    return ResponseResult.PostResult(
                        result: false,
                        status: HttpStatusCode.BadRequest,
                        exception: null,
                        message: "يوجد تداخل في التواريخ والوحدات مع طلب موافق عليه. عدّل وحدات الطلب المرن قبل الموافقة.");
                }
            }

            return null;
        }

        private static bool RequestDateRangesOverlap(Request left, Request right) =>
            left.StartDate < right.EndDate && right.StartDate < left.EndDate;

        private async Task<List<Request>> RejectConflictingFixedRequestsAsync(Request approvedRequest, CancellationToken cancellationToken)
        {
            IEnumerable<Request> candidates = await UnitOfWork.Repository.FindAsync(
                predicate: r => !r.IsDeleted
                    && r.Id != approvedRequest.Id
                    && r.RequestAllocationType == AllocationType.Fixed
                    && r.Status == Status.Pending
                    && r.StartDate >= approvedRequest.StartDate
                    && r.StartDate < approvedRequest.EndDate,
                include: src => src
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Bed).ThenInclude(b => b.Room)
                    .Include(r => r.RequestUnits).ThenInclude(u => u.Room),
                disableTracking: false,
                cancellationToken: cancellationToken);

            List<RequestUnit> approvedUnits = approvedRequest.RequestUnits
                .Where(u => !u.IsDeleted)
                .ToList();
            if (approvedUnits.Count == 0)
                return [];

            List<Request> rejectedRequests = [];

            List<Request> candidateList = candidates.ToList();
            List<RequestUnit> allUnits = [.. approvedUnits];
            foreach (Request candidate in candidateList)
                allUnits.AddRange(candidate.RequestUnits.Where(u => !u.IsDeleted));

            IReadOnlyDictionary<string, string> bedRoomIds =
                await GetBedRoomIdsAsync(allUnits, cancellationToken);
            IReadOnlyDictionary<string, string> roomApartmentIds =
                await GetRoomApartmentIdsAsync(allUnits, bedRoomIds, cancellationToken);

            string rejectionReason =
                $"تم إلغاء الحجز بسبب اولوية طلبات سابقة";

            foreach (Request candidate in candidateList)
            {
                List<RequestUnit> candidateUnits = candidate.RequestUnits
                    .Where(u => !u.IsDeleted)
                    .ToList();
                if (candidateUnits.Count == 0
                    || !RequestUnitsOverlap(approvedUnits, candidateUnits, bedRoomIds, roomApartmentIds))
                    continue;

                candidate.Status = Status.Rejected;
                candidate.RejectionReason = rejectionReason;
                SetEntityModifiedBaseProperties(candidate);
                rejectedRequests.Add(candidate);
            }

            return rejectedRequests;
        }

        private async Task TrySendRequestStatusEmailAsync(Request request, CancellationToken cancellationToken)
        {
            if (request.Status is not (Status.Approved or Status.Rejected))
                return;

            User owner = await UnitOfWork.GetRepository<User>().FirstOrDefaultAsync(
                u => u.Id == request.UserId,
                disableTracking: true,
                cancellationToken: cancellationToken);

            if (owner == null || string.IsNullOrWhiteSpace(owner.Email))
                return;

            try
            {
                bool isApproved = request.Status == Status.Approved;
                string subject = isApproved
                    ? "قبول طلب الحجز - نظام حجز الإسكان"
                    : "رفض طلب الحجز - نظام حجز الإسكان";
                string statusText = isApproved ? "مقبول" : "مرفوض";
                string rejectionSection = !isApproved
                    ? $"<p><strong>سبب الرفض:</strong> {WebUtility.HtmlEncode(
                        string.IsNullOrWhiteSpace(request.RejectionReason) ? "لم يتم تحديد سبب" : request.RejectionReason)}</p>"
                    : string.Empty;

                string body = $"""
                    <div dir="rtl" style="font-family: Arial, sans-serif;">
                    <h2>مرحباً {WebUtility.HtmlEncode(owner.FullName)}</h2>
                    <p>طلب الحجز رقم <strong>{WebUtility.HtmlEncode(request.RequestNumber)}</strong> أصبح الآن <strong>{statusText}</strong>.</p>
                    <p><strong>تاريخ الوصول:</strong> {request.StartDate:dd/MM/yyyy}</p>
                    <p><strong>تاريخ المغادرة:</strong> {request.EndDate:dd/MM/yyyy}</p>
                    {rejectionSection}
                    </div>
                    """;

                await emailService.SendEmailAsync(owner.Email, subject, body);
            }
            catch
            {
                // Status change is already persisted; do not fail the update when email delivery fails.
            }
        }

        private async Task<IReadOnlyDictionary<string, string>> GetBedRoomIdsAsync(
            IEnumerable<RequestUnit> units,
            CancellationToken cancellationToken)
        {
            HashSet<string> bedIds = new(StringComparer.OrdinalIgnoreCase);
            foreach (RequestUnit unit in units)
            {
                if (!string.IsNullOrWhiteSpace(unit.BedId))
                    bedIds.Add(unit.BedId.Trim());
            }

            if (bedIds.Count == 0)
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            IEnumerable<Bed> beds = await UnitOfWork.GetRepository<Bed>().FindAsync(
                predicate: b => bedIds.Contains(b.Id),
                disableTracking: true,
                cancellationToken: cancellationToken);

            return beds.ToDictionary(b => b.Id, b => b.RoomId, StringComparer.OrdinalIgnoreCase);
        }

        private async Task<IReadOnlyDictionary<string, string>> GetRoomApartmentIdsAsync(
            IEnumerable<RequestUnit> units,
            IReadOnlyDictionary<string, string> bedRoomIds,
            CancellationToken cancellationToken)
        {
            HashSet<string> roomIds = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> apartmentIds = new(StringComparer.OrdinalIgnoreCase);

            foreach (RequestUnit unit in units)
            {
                UnitScope scope = ResolveUnitScope(unit, bedRoomIds, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(scope.RoomId))
                    roomIds.Add(scope.RoomId);
                if (scope.Granularity == UnitBookingGranularity.Apartment
                    && !string.IsNullOrWhiteSpace(scope.ApartmentId))
                {
                    apartmentIds.Add(scope.ApartmentId);
                }
            }

            if (apartmentIds.Count > 0)
            {
                IEnumerable<Room> childRooms = await UnitOfWork.GetRepository<Room>().FindAsync(
                    predicate: r => apartmentIds.Contains(r.ApartmentId),
                    disableTracking: true,
                    cancellationToken: cancellationToken);

                foreach (Room room in childRooms)
                    roomIds.Add(room.Id);
            }

            if (roomIds.Count == 0)
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            IEnumerable<Room> rooms = await UnitOfWork.GetRepository<Room>().FindAsync(
                predicate: r => roomIds.Contains(r.Id),
                disableTracking: true,
                cancellationToken: cancellationToken);

            return rooms.ToDictionary(r => r.Id, r => r.ApartmentId, StringComparer.OrdinalIgnoreCase);
        }

        private static bool RequestUnitsOverlap(
            IReadOnlyList<RequestUnit> approvedUnits,
            IReadOnlyList<RequestUnit> candidateUnits,
            IReadOnlyDictionary<string, string> bedRoomIds,
            IReadOnlyDictionary<string, string> roomApartmentIds)
        {
            foreach (RequestUnit approvedUnit in approvedUnits)
            {
                UnitScope approvedScope = ResolveUnitScope(approvedUnit, bedRoomIds, roomApartmentIds);
                foreach (RequestUnit candidateUnit in candidateUnits)
                {
                    UnitScope candidateScope = ResolveUnitScope(candidateUnit, bedRoomIds, roomApartmentIds);
                    if (UnitScopesHierarchicallyConflict(approvedScope, candidateScope, bedRoomIds, roomApartmentIds))
                        return true;
                }
            }

            return false;
        }

        private static bool UnitScopesHierarchicallyConflict(
            UnitScope left,
            UnitScope right,
            IReadOnlyDictionary<string, string> bedRoomIds,
            IReadOnlyDictionary<string, string> roomApartmentIds)
        {
            if (ScopesSameUnit(left, right))
                return true;

            return IsDescendantScope(left, right, bedRoomIds, roomApartmentIds)
                || IsDescendantScope(right, left, bedRoomIds, roomApartmentIds);
        }

        private static bool ScopesSameUnit(UnitScope left, UnitScope right)
        {
            if (left.Granularity == UnitBookingGranularity.Bed && right.Granularity == UnitBookingGranularity.Bed)
            {
                return !string.IsNullOrWhiteSpace(left.BedId)
                    && !string.IsNullOrWhiteSpace(right.BedId)
                    && string.Equals(left.BedId, right.BedId, StringComparison.OrdinalIgnoreCase);
            }

            if (left.Granularity == UnitBookingGranularity.Room && right.Granularity == UnitBookingGranularity.Room)
            {
                return !string.IsNullOrWhiteSpace(left.RoomId)
                    && !string.IsNullOrWhiteSpace(right.RoomId)
                    && string.Equals(left.RoomId, right.RoomId, StringComparison.OrdinalIgnoreCase);
            }

            if (left.Granularity == UnitBookingGranularity.Apartment && right.Granularity == UnitBookingGranularity.Apartment)
            {
                return !string.IsNullOrWhiteSpace(left.ApartmentId)
                    && !string.IsNullOrWhiteSpace(right.ApartmentId)
                    && string.Equals(left.ApartmentId, right.ApartmentId, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool IsDescendantScope(
            UnitScope ancestor,
            UnitScope descendant,
            IReadOnlyDictionary<string, string> bedRoomIds,
            IReadOnlyDictionary<string, string> roomApartmentIds)
        {
            return ancestor.Granularity switch
            {
                UnitBookingGranularity.Apartment =>
                    !string.IsNullOrWhiteSpace(ancestor.ApartmentId)
                    && string.Equals(
                        ancestor.ApartmentId,
                        ResolveApartmentId(descendant, bedRoomIds, roomApartmentIds),
                        StringComparison.OrdinalIgnoreCase),
                UnitBookingGranularity.Room =>
                    !string.IsNullOrWhiteSpace(ancestor.RoomId)
                    && string.Equals(
                        ancestor.RoomId,
                        ResolveRoomId(descendant, bedRoomIds),
                        StringComparison.OrdinalIgnoreCase),
                UnitBookingGranularity.Bed =>
                    descendant.Granularity == UnitBookingGranularity.Bed
                    && !string.IsNullOrWhiteSpace(ancestor.BedId)
                    && !string.IsNullOrWhiteSpace(descendant.BedId)
                    && string.Equals(ancestor.BedId, descendant.BedId, StringComparison.OrdinalIgnoreCase),
                _ => false
            };
        }

        private static string ResolveApartmentId(
            UnitScope scope,
            IReadOnlyDictionary<string, string> bedRoomIds,
            IReadOnlyDictionary<string, string> roomApartmentIds)
        {
            if (!string.IsNullOrWhiteSpace(scope.ApartmentId))
                return scope.ApartmentId.Trim();

            string roomId = ResolveRoomId(scope, bedRoomIds);
            if (string.IsNullOrWhiteSpace(roomId))
                return null;

            return roomApartmentIds.TryGetValue(roomId, out string apartmentId)
                ? apartmentId
                : null;
        }

        private static string ResolveRoomId(
            UnitScope scope,
            IReadOnlyDictionary<string, string> bedRoomIds)
        {
            if (!string.IsNullOrWhiteSpace(scope.RoomId))
                return scope.RoomId.Trim();

            if (string.IsNullOrWhiteSpace(scope.BedId))
                return null;

            return bedRoomIds.TryGetValue(scope.BedId.Trim(), out string roomId)
                ? roomId
                : null;
        }

        private static UnitScope ResolveUnitScope(
            RequestUnit unit,
            IReadOnlyDictionary<string, string> bedRoomIds,
            IReadOnlyDictionary<string, string> roomApartmentIds)
        {
            string bedId = string.IsNullOrWhiteSpace(unit.BedId) ? null : unit.BedId.Trim();

            string roomId = string.IsNullOrWhiteSpace(unit.RoomId) ? null : unit.RoomId.Trim();
            if (string.IsNullOrWhiteSpace(roomId) && !string.IsNullOrWhiteSpace(bedId))
            {
                if (unit.Bed?.RoomId is { Length: > 0 } bedRoomId)
                    roomId = bedRoomId.Trim();
                else if (bedRoomIds.TryGetValue(bedId, out string mappedRoomId))
                    roomId = mappedRoomId;
            }

            string apartmentId = string.IsNullOrWhiteSpace(unit.ApartmentId) ? null : unit.ApartmentId.Trim();
            if (string.IsNullOrWhiteSpace(apartmentId) && unit.Room?.ApartmentId is { Length: > 0 } roomApartmentId)
                apartmentId = roomApartmentId.Trim();
            if (string.IsNullOrWhiteSpace(apartmentId) && unit.Bed?.Room?.ApartmentId is { Length: > 0 } bedApartmentId)
                apartmentId = bedApartmentId.Trim();
            if (string.IsNullOrWhiteSpace(apartmentId)
                && !string.IsNullOrWhiteSpace(roomId)
                && roomApartmentIds.TryGetValue(roomId, out string mappedApartmentId))
                apartmentId = mappedApartmentId;

            UnitBookingGranularity granularity;
            if (!string.IsNullOrWhiteSpace(bedId))
                granularity = UnitBookingGranularity.Bed;
            else if (!string.IsNullOrWhiteSpace(roomId))
                granularity = UnitBookingGranularity.Room;
            else if (!string.IsNullOrWhiteSpace(apartmentId))
                granularity = UnitBookingGranularity.Apartment;
            else
                granularity = UnitBookingGranularity.Bed;

            return new UnitScope(granularity, bedId, roomId, apartmentId);
        }

        private enum UnitBookingGranularity
        {
            Bed = 1,
            Room = 2,
            Apartment = 3
        }

        private readonly struct UnitScope(
            UnitBookingGranularity granularity,
            string bedId,
            string roomId,
            string apartmentId)
        {
            public UnitBookingGranularity Granularity { get; } = granularity;
            public string BedId { get; } = bedId;
            public string RoomId { get; } = roomId;
            public string ApartmentId { get; } = apartmentId;
        }

        private async Task<IFinalResult?> ValidateRequestToLeaderAsync(string requestToId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(requestToId))
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "RequestToId is required.");

            Leader? leader = await UnitOfWork.GetRepository<Leader>().FirstOrDefaultAsync(
                x => x.Id == requestToId.Trim() && !x.IsDeleted && x.IsActive,
                disableTracking: true,
                cancellationToken: cancellationToken);

            if (leader == null)
                return ResponseResult.PostResult(result: null, status: HttpStatusCode.BadRequest, exception: null,
                    message: "Invalid or inactive leader.");

            return null;
        }

        private async Task AddRequestAttachmentsAsync(
            Request entity,
            List<AddRequestAttachDto> images,
            CancellationToken cancellationToken)
        {
            if (images == null || images.Count == 0)
                return;

            foreach (AddRequestAttachDto formFile in images)
            {
                if (formFile?.Image == null || formFile.Image.Length <= 0)
                    continue;

                var addDto = new AddAttachmentDto
                {
                    Id = Guid.NewGuid().ToString(),
                    File = formFile.Image,
                    AttachFolder = "Requests",
                };

                IFinalResult attach = await attachmentService.AddAsync(addDto, cancellationToken);
                if (attach?.Data == null)
                    continue;

                string attachmentId = attach.Data.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(attachmentId))
                    continue;

                entity.RequestAttaches.Add(new RequestAttach
                {
                    AttachmentId = attachmentId,
                    RequestId = entity.Id,
                    IsPrimary = formFile.IsPrimary
                });
            }
        }
    }
}
