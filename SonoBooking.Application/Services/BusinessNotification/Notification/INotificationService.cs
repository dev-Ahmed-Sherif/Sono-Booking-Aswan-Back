using SonoBooking.Common.Core;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.BusinessNotification.Notification
{
    public interface INotificationService
    {
        Task<IFinalResult> GetNotificationsAsync(
            int take = 20,
            bool onlyUnread = false,
            CancellationToken cancellationToken = default);

        Task<IFinalResult> GetUnreadCountAsync(CancellationToken cancellationToken = default);

        Task<IFinalResult> MarkAsReadAsync(
            string notificationId,
            CancellationToken cancellationToken = default);

        Task<IFinalResult> MarkAllAsReadAsync(CancellationToken cancellationToken = default);

        Task CreateAndPublishAsync(
            string receiverId,
            string senderId,
            string type,
            string content,
            string? referenceId = null,
            string? senderName = null,
            string? notificationGroupCode = null,
            CancellationToken cancellationToken = default);
    }
}
