using Microsoft.AspNetCore.SignalR;
using SonoBooking.Api.Hubs;
using SonoBooking.Application.Services.BusinessNotification.Chat;
using SonoBooking.Common.DTO.BusinessNotification.Chat;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Api.Services.BusinessNotification
{
    public class ChatRealtimePublisher(IHubContext<ChatHub> hubContext) : IChatRealtimePublisher
    {
        public Task PublishMessageAsync(
            ChatMessageDto message,
            IReadOnlyList<string> participantUserIds,
            CancellationToken cancellationToken = default)
        {
            var distinctParticipantIds = participantUserIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct()
                .ToList();

            if (distinctParticipantIds.Count == 0)
            {
                return Task.CompletedTask;
            }

            var deliveries = distinctParticipantIds.Select(userId =>
                hubContext.Clients
                    .User(userId)
                    .SendAsync("ReceiveMessage", message, cancellationToken));

            return Task.WhenAll(deliveries);
        }

        public Task PublishConversationUpdatedAsync(
            string userId,
            ChatConversationUpdatedDto update,
            CancellationToken cancellationToken = default)
        {
            return hubContext.Clients
                .User(userId)
                .SendAsync("ConversationUpdated", update, cancellationToken);
        }

        public Task PublishChatUnreadCountAsync(
            string userId,
            int unreadCount,
            CancellationToken cancellationToken = default)
        {
            return hubContext.Clients
                .User(userId)
                .SendAsync("ChatUnreadCountUpdated", unreadCount, cancellationToken);
        }

        public Task PublishUserPresenceChangedAsync(
            string userId,
            bool isOnline,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Task.CompletedTask;
            }

            return hubContext.Clients
                .All
                .SendAsync("UserPresenceChanged", userId, isOnline, cancellationToken);
        }
    }
}
