using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SonoBooking.Common.Constants.BusinessNotification;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.BusinessNotification.Notification;
using SonoBooking.Common.DTO.Identity.User;
using SonoBooking.Domain;
using SonoBooking.Infrastructure.Context;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.BusinessNotification.Notification
{
    public class NotificationService(
        SonoBookingDbContext context,
        UserDataDto currentUser,
        IHttpContextAccessor httpContextAccessor,
        INotificationRealtimePublisher realtimePublisher) : INotificationService
    {
        private const int DefaultPageSize = 20;
        private const int MaxPageSize = 100;

        public async Task<IFinalResult> GetNotificationsAsync(
            int take = DefaultPageSize,
            bool onlyUnread = false,
            CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            take = Math.Clamp(take, 1, MaxPageSize);

            var query = context.Notifications
                .AsNoTracking()
                .Where(n => !n.IsDeleted && n.ReceiverId == currentUser.Id);

            if (onlyUnread)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);

            var senderIds = notifications.Select(n => n.SenderId).Distinct().ToList();
            var senderNames = await context.Users
                .AsNoTracking()
                .Where(u => senderIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

            var groupIds = notifications
                .Where(n => !string.IsNullOrWhiteSpace(n.NotificationGroupId))
                .Select(n => n.NotificationGroupId!)
                .Distinct()
                .ToList();

            var groups = groupIds.Count == 0
                ? new System.Collections.Generic.Dictionary<string, Domain.Entities.BusinessNotification.NotificationGroup>()
                : await context.Set<Domain.Entities.BusinessNotification.NotificationGroup>()
                    .AsNoTracking()
                    .Where(g => groupIds.Contains(g.Id!))
                    .ToDictionaryAsync(g => g.Id!, cancellationToken);

            var dtos = notifications
                .Select(n => ToDto(n, senderNames, groups))
                .ToList();

            return response.PostResult(dtos, HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public async Task<IFinalResult> GetUnreadCountAsync(CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            var count = await context.Notifications
                .AsNoTracking()
                .CountAsync(n =>
                    !n.IsDeleted &&
                    n.ReceiverId == currentUser.Id &&
                    !n.IsRead,
                    cancellationToken);

            return response.PostResult(new { count }, HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public async Task<IFinalResult> MarkAsReadAsync(
            string notificationId,
            CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            if (string.IsNullOrWhiteSpace(notificationId))
            {
                return response.PostResult(null, HttpStatusCode.BadRequest, message: "notificationId is required.");
            }

            var notification = await context.Notifications
                .FirstOrDefaultAsync(n =>
                    n.Id == notificationId &&
                    !n.IsDeleted &&
                    n.ReceiverId == currentUser.Id,
                    cancellationToken);

            if (notification == null)
            {
                return response.PostResult(null, HttpStatusCode.NotFound, message: "Notification not found.");
            }

            if (!notification.IsRead)
            {
                var now = DateTime.UtcNow;
                notification.IsRead = true;
                notification.ModifiedAt = now;
                notification.ModifiedById = currentUser.Id;
                notification.ModifiedBy = currentUser.Name;
                await context.SaveChangesAsync(cancellationToken);

                var unreadCount = await GetUnreadCountForUserAsync(currentUser.Id, cancellationToken);
                await realtimePublisher.PublishUnreadCountAsync(currentUser.Id, unreadCount, cancellationToken);
            }

            return response.PostResult(true, HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public async Task<IFinalResult> MarkAllAsReadAsync(CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            var unread = await context.Notifications
                .Where(n =>
                    !n.IsDeleted &&
                    n.ReceiverId == currentUser.Id &&
                    !n.IsRead)
                .ToListAsync(cancellationToken);

            if (unread.Count > 0)
            {
                var now = DateTime.UtcNow;
                foreach (var notification in unread)
                {
                    notification.IsRead = true;
                    notification.ModifiedAt = now;
                    notification.ModifiedById = currentUser.Id;
                    notification.ModifiedBy = currentUser.Name;
                }

                await context.SaveChangesAsync(cancellationToken);
                await realtimePublisher.PublishUnreadCountAsync(currentUser.Id, 0, cancellationToken);
            }

            return response.PostResult(true, HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public async Task CreateAndPublishAsync(
            string receiverId,
            string senderId,
            string type,
            string content,
            string? referenceId = null,
            string? senderName = null,
            string? notificationGroupCode = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(receiverId) ||
                string.IsNullOrWhiteSpace(senderId) ||
                string.IsNullOrWhiteSpace(content))
            {
                return;
            }

            var normalizedType = string.IsNullOrWhiteSpace(type)
                ? NotificationTypes.System
                : type.Trim().ToLowerInvariant();

            Domain.Entities.BusinessNotification.NotificationGroup? notificationGroup = null;
            if (!string.IsNullOrWhiteSpace(notificationGroupCode))
            {
                notificationGroup = await context.Set<Domain.Entities.BusinessNotification.NotificationGroup>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(g =>
                        !g.IsDeleted &&
                        g.Code == notificationGroupCode.Trim(),
                        cancellationToken);
            }

            var now = DateTime.UtcNow;
            var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            if (string.IsNullOrWhiteSpace(senderName))
            {
                senderName = await context.Users
                    .AsNoTracking()
                    .Where(u => u.Id == senderId)
                    .Select(u => u.FullName)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var notification = new Domain.Entities.BusinessNotification.Notification
            {
                Content = content.Trim(),
                Type = normalizedType,
                ReferenceId = referenceId,
                NotificationGroupId = notificationGroup?.Id,
                SenderId = senderId,
                ReceiverId = receiverId,
                IsRead = false,
                GovernorateId = string.IsNullOrWhiteSpace(currentUser.GovernorateId)
                    ? null
                    : currentUser.GovernorateId,
                CreatedAt = now,
                CreatedById = senderId,
                CreatedBy = senderName ?? senderId,
                ModifiedAt = now,
                ModifiedById = senderId,
                ModifiedBy = senderName ?? senderId,
                IpAddress = ip
            };

            context.Notifications.Add(notification);
            await context.SaveChangesAsync(cancellationToken);

            var dto = ToDto(
                notification,
                new System.Collections.Generic.Dictionary<string, string>
                {
                    [senderId] = senderName ?? senderId
                },
                notificationGroup == null
                    ? null
                    : new System.Collections.Generic.Dictionary<string, Domain.Entities.BusinessNotification.NotificationGroup>
                    {
                        [notificationGroup.Id!] = notificationGroup
                    });

            await realtimePublisher.PublishNotificationAsync(receiverId, dto, cancellationToken);

            var unreadCount = await GetUnreadCountForUserAsync(receiverId, cancellationToken);
            await realtimePublisher.PublishUnreadCountAsync(receiverId, unreadCount, cancellationToken);
        }

        private async Task<int> GetUnreadCountForUserAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            return await context.Notifications
                .AsNoTracking()
                .CountAsync(n =>
                    !n.IsDeleted &&
                    n.ReceiverId == userId &&
                    !n.IsRead,
                    cancellationToken);
        }

        private static NotificationDto ToDto(
            Domain.Entities.BusinessNotification.Notification notification,
            System.Collections.Generic.IReadOnlyDictionary<string, string> senderNames,
            System.Collections.Generic.IReadOnlyDictionary<string, Domain.Entities.BusinessNotification.NotificationGroup>? groups = null)
        {
            senderNames.TryGetValue(notification.SenderId, out var senderName);

            Domain.Entities.BusinessNotification.NotificationGroup? group = null;
            if (!string.IsNullOrWhiteSpace(notification.NotificationGroupId) &&
                groups != null)
            {
                groups.TryGetValue(notification.NotificationGroupId, out group);
            }

            return new NotificationDto
            {
                Id = notification.Id!,
                Type = notification.Type,
                Content = notification.Content,
                ReferenceId = notification.ReferenceId,
                GroupType = group?.Code,
                GroupNameAr = group?.NameAr,
                GroupNameEn = group?.NameEn,
                SenderId = notification.SenderId,
                SenderName = senderName,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}
