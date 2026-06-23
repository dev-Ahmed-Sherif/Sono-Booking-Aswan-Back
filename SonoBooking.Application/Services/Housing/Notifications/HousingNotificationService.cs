using Microsoft.EntityFrameworkCore;

using SonoBooking.Application.Services.BusinessNotification.Notification;

using SonoBooking.Common.Constants.Auth;

using SonoBooking.Common.Constants.BusinessNotification;

using SonoBooking.Domain;

using SonoBooking.Domain.Entities.Housing;

using SonoBooking.Domain.Entities.Identity;

using SonoBooking.Infrastructure.Context;

using System;

using System.Collections.Generic;

using System.Linq;

using System.Threading;

using System.Threading.Tasks;



namespace SonoBooking.Application.Services.Housing.Notifications

{

    public class HousingNotificationService(

        SonoBookingDbContext context,

        INotificationService notificationService)

    {

        public async Task NotifyLeadersOnNewRequestAsync(

            Request request,

            CancellationToken cancellationToken = default)

        {

            if (request == null || string.IsNullOrWhiteSpace(request.UserId))

                return;



            User? owner = request.User;

            if (owner == null)

            {

                owner = await context.Users

                    .AsNoTracking()

                    .FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken);

            }



            string ownerName = owner?.FullName?.Trim();

            if (string.IsNullOrWhiteSpace(ownerName))

                ownerName = "صاحب الطلب";



            string content = HousingNotificationMessages.NewRequest(

                request.RequestNumber,

                ownerName,

                request.RequestDate == default ? DateTime.UtcNow : request.RequestDate);



            List<string> leaderIds = await GetActiveUserIdsInRoleAsync(RoleNames.Leader, cancellationToken);

            foreach (string leaderId in leaderIds)

            {

                if (string.Equals(leaderId, request.UserId, StringComparison.Ordinal))

                    continue;



                await TryPublishAsync(

                    leaderId,

                    request.UserId,

                    NotificationTypes.Request,

                    content,

                    referenceId: request.Id,

                    senderName: ownerName,

                    cancellationToken: cancellationToken);

            }

        }



        public async Task NotifyOwnerOnRequestDecisionAsync(

            Request request,

            string? actorName,

            CancellationToken cancellationToken = default)

        {

            if (request == null || string.IsNullOrWhiteSpace(request.UserId))

                return;



            if (request.Status is not (Status.Approved or Status.Rejected))

                return;



            string content = request.Status == Status.Approved

                ? HousingNotificationMessages.RequestApproved(

                    request.RequestNumber,

                    string.IsNullOrWhiteSpace(actorName) ? "القائد" : actorName.Trim())

                : HousingNotificationMessages.RequestRejected(request.RequestNumber);



            string senderId = request.ApprovedById?.Trim() ?? request.ModifiedById?.Trim() ?? request.UserId;

            string type = NotificationTypes.Request;



            await TryPublishAsync(

                request.UserId,

                senderId,

                type,

                content,

                referenceId: request.Id,

                senderName: actorName,

                cancellationToken: cancellationToken);

        }



        public async Task NotifyReceptionOnNewReservationAsync(

            Reservation reservation,

            CancellationToken cancellationToken = default)

        {

            if (reservation == null || string.IsNullOrWhiteSpace(reservation.RequestId))

                return;



            Request? request = reservation.Request;

            if (request == null)

            {

                request = await context.Requests

                    .AsNoTracking()

                    .FirstOrDefaultAsync(r => r.Id == reservation.RequestId && !r.IsDeleted, cancellationToken);

            }



            if (request == null)

                return;



            string content = HousingNotificationMessages.NewReservationForReception(

                request.RequestNumber,

                reservation.StartDate,

                reservation.EndDate);



            string senderId = reservation.CreatedById?.Trim() ?? request.UserId;

            string? senderName = reservation.CreatedBy?.Trim();



            List<string> receptionIds = await GetActiveUserIdsInRoleAsync(RoleNames.ReceptionStaff, cancellationToken);

            foreach (string receptionId in receptionIds)

            {

                await TryPublishAsync(

                    receptionId,

                    senderId,

                    NotificationTypes.Reservation,

                    content,

                    referenceId: reservation.Id,

                    senderName: senderName,

                    cancellationToken: cancellationToken);

            }

        }



        public async Task NotifyOwnerOnReservationStatusChangeAsync(

            Reservation reservation,

            ReservationStatus previousStatus,

            CancellationToken cancellationToken = default)

        {

            if (reservation == null || reservation.Status == previousStatus)

                return;



            Request? request = reservation.Request;

            if (request == null)

            {

                request = await context.Requests

                    .AsNoTracking()

                    .FirstOrDefaultAsync(r => r.Id == reservation.RequestId && !r.IsDeleted, cancellationToken);

            }



            if (request == null || string.IsNullOrWhiteSpace(request.UserId))

                return;



            string content = HousingNotificationMessages.ReservationStatusUpdated(

                request.RequestNumber,

                reservation.Status);



            await TryPublishAsync(

                request.UserId,

                reservation.ModifiedById?.Trim() ?? request.UserId,

                NotificationTypes.Reservation,

                content,

                referenceId: reservation.Id,

                senderName: reservation.ModifiedBy,

                cancellationToken: cancellationToken);

        }



        private async Task<List<string>> GetActiveUserIdsInRoleAsync(

            string roleName,

            CancellationToken cancellationToken)

        {

            return await (

                from userRole in context.UserRoles

                join role in context.Roles on userRole.RoleId equals role.Id

                join user in context.Users on userRole.UserId equals user.Id

                where role.Name == roleName && !user.IsDeleted

                select user.Id

            ).Distinct().ToListAsync(cancellationToken);

        }



        private async Task TryPublishAsync(

            string receiverId,

            string senderId,

            string type,

            string content,

            string? referenceId,

            string? senderName,

            CancellationToken cancellationToken)

        {

            try

            {

                await notificationService.CreateAndPublishAsync(

                    receiverId,

                    senderId,

                    type,

                    content,

                    referenceId: referenceId,

                    senderName: senderName,

                    cancellationToken: cancellationToken);

            }

            catch

            {

                // Do not fail the business operation when notification delivery fails.

            }

        }

    }

}


