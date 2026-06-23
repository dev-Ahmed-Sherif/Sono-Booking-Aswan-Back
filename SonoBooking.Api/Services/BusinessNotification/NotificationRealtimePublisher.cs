using Microsoft.AspNetCore.SignalR;
using SonoBooking.Api.Hubs;
using SonoBooking.Application.Services.BusinessNotification.Notification;
using SonoBooking.Common.DTO.BusinessNotification.Notification;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Api.Services.BusinessNotification
{
    public class NotificationRealtimePublisher(IHubContext<NotificationHub> hubContext)
        : INotificationRealtimePublisher
    {
        public Task PublishNotificationAsync(
            string userId,
            NotificationDto notification,
            CancellationToken cancellationToken = default)
        {
            return hubContext.Clients
                .User(userId)
                .SendAsync("ReceiveNotification", notification, cancellationToken);
        }

        public Task PublishUnreadCountAsync(
            string userId,
            int unreadCount,
            CancellationToken cancellationToken = default)
        {
            return hubContext.Clients
                .User(userId)
                .SendAsync("UnreadCountUpdated", unreadCount, cancellationToken);
        }
    }
}
