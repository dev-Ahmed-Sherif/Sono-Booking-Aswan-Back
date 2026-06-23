using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SonoBooking.Api.Hubs
{
    /// <summary>
    /// Realtime notification hub for user-targeted alerts.
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
