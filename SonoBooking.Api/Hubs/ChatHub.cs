using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SonoBooking.Application.Services.BusinessNotification.Chat;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.BusinessNotification.Chat;
using System.Net;
using System.Security.Claims;

namespace SonoBooking.Api.Hubs
{
    /// <summary>
    /// Realtime chat hub for conversation messaging and live updates.
    /// </summary>
    [Authorize]
    public class ChatHub(IChatService chatService) : Hub
    {
        /// <summary>
        /// Returns online status for the requested user ids (initial load).
        /// </summary>
        public async Task<UserOnlineStatusDto[]> GetOnlineStatuses(string[] userIds)
        {
            IFinalResult result = await chatService.GetOnlineStatusesAsync(
                userIds ?? Array.Empty<string>(),
                Context.ConnectionAborted);

            if (result.Status != HttpStatusCode.OK)
            {
                return Array.Empty<UserOnlineStatusDto>();
            }

            if (result.Data is UserOnlineStatusDto[] statuses)
            {
                return statuses;
            }

            if (result.Data is IEnumerable<UserOnlineStatusDto> enumerable)
            {
                return enumerable.ToArray();
            }

            return Array.Empty<UserOnlineStatusDto>();
        }

        /// <summary>
        /// Adds the connection to a conversation group after verifying membership.
        /// </summary>
        public async Task JoinConversation(string conversationId)
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            if (!await chatService.EnsureParticipantAsync(conversationId, userId, Context.ConnectionAborted))
            {
                throw new HubException("Forbidden: not a participant in this conversation.");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId, Context.ConnectionAborted);
        }

        /// <summary>
        /// Removes the connection from a conversation group.
        /// </summary>
        public Task LeaveConversation(string conversationId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId, Context.ConnectionAborted);
        }

        /// <summary>
        /// Sends a message to a conversation and broadcasts it to group members.
        /// </summary>
        public async Task SendMessage(string conversationId, string content)
        {
            IFinalResult result = await chatService.SendMessageAsync(
                conversationId,
                content,
                publishRealtime: true,
                cancellationToken: Context.ConnectionAborted);

            if (result.Status != HttpStatusCode.OK)
            {
                throw new HubException(result.Message ?? "Failed to send message.");
            }
        }

        private string? GetUserId()
        {
            return Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
