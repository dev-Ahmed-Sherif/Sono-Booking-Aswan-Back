using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.BusinessNotification.Chat;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.BusinessNotification.Chat
{
    public interface IChatService
    {
        Task<IFinalResult> GetConversationsAsync(CancellationToken cancellationToken = default);

        Task<IFinalResult> GetChatContactsAsync(CancellationToken cancellationToken = default);

        Task<IFinalResult> GetMessagesAsync(
            string conversationId,
            int take = 50,
            string? beforeMessageId = null,
            CancellationToken cancellationToken = default);

        Task<IFinalResult> CreateConversationAsync(
            IReadOnlyList<string> participantUserIds,
            CancellationToken cancellationToken = default);

        Task<IFinalResult> SendMessageAsync(
            string conversationId,
            string content,
            bool publishRealtime = true,
            CancellationToken cancellationToken = default);

        Task<bool> EnsureParticipantAsync(
            string conversationId,
            string userId,
            CancellationToken cancellationToken = default);

        Task<IFinalResult> GetOrCreateRequestConversationAsync(
            string requestId,
            string groupType,
            CancellationToken cancellationToken = default);

        Task<IFinalResult> GetRequestConversationsAsync(
            string requestId,
            CancellationToken cancellationToken = default);

        Task<IFinalResult> GetOnlineStatusesAsync(
            IReadOnlyList<string> userIds,
            CancellationToken cancellationToken = default);
    }
}
