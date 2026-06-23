using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SonoBooking.Api.Hubs
{
    /// <summary>
    /// WebRTC signaling hub for 1:1 video calls (offer, answer, ICE, end).
    /// </summary>
    [Authorize]
    public class VideoChatHub : Hub
    {
        /// <summary>
        /// Sends a WebRTC session offer to the target user.
        /// </summary>
        public Task SendOffer(string receiverId, string offer)
        {
            return Clients.User(receiverId).SendAsync("ReceiveOffer", Context.UserIdentifier, offer);
        }

        /// <summary>
        /// Sends a WebRTC session answer to the target user.
        /// </summary>
        public Task SendAnswer(string receiverId, string answer)
        {
            return Clients.User(receiverId).SendAsync("ReceiveAnswer", Context.UserIdentifier, answer);
        }

        /// <summary>
        /// Forwards an ICE candidate to the target user.
        /// </summary>
        public Task SendIceCandidate(string receiverId, string candidate)
        {
            return Clients.User(receiverId).SendAsync("ReceiveIceCandidate", Context.UserIdentifier, candidate);
        }

        /// <summary>
        /// Notifies the remote participant that the call has ended.
        /// </summary>
        public Task EndCall(string receiverId)
        {
            return Clients.User(receiverId).SendAsync("CallEnded");
        }
    }
}
