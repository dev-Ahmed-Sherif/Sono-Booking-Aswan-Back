using System.Collections.Generic;

namespace SonoBooking.Application.Services.BusinessNotification.Chat
{
    /// <summary>
    /// Tracks chat user online presence by active SignalR connection count.
    /// In-memory only; use Redis or similar for multi-server deployments.
    /// </summary>
    public interface IUserPresenceTracker
    {
        bool IsOnline(string userId);

        IReadOnlyCollection<string> GetOnlineUserIds();

        /// <summary>
        /// Registers a connection. Returns true when the user transitions to online.
        /// </summary>
        bool OnConnected(string userId);

        /// <summary>
        /// Unregisters a connection. Returns true when the user transitions to offline.
        /// </summary>
        bool OnDisconnected(string userId);
    }
}
