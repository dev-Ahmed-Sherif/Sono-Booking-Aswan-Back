using SonoBooking.Common.DTO.BusinessNotification.Notification;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.BusinessNotification.Notification
{
    public interface INotificationRealtimePublisher
    {
        Task PublishNotificationAsync(
            string userId,
            NotificationDto notification,
            CancellationToken cancellationToken = default);

        Task PublishUnreadCountAsync(
            string userId,
            int unreadCount,
            CancellationToken cancellationToken = default);
    }
}
