using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SonoBooking.Api.Hubs
{
    /// <summary>
    /// Maps authenticated users to SignalR user identifiers for targeted messaging.
    /// </summary>
    public class SignalRUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
