using SonoBooking.Application.Services.BusinessNotification.Chat;
using System.Collections.Concurrent;

namespace SonoBooking.Api.Services.BusinessNotification
{
    /// <summary>
    /// In-memory presence tracker keyed by user id with per-user connection counts.
    /// </summary>
    public class InMemoryUserPresenceTracker : IUserPresenceTracker
    {
        private readonly ConcurrentDictionary<string, int> _connectionCounts = new(StringComparer.Ordinal);

        public bool IsOnline(string userId)
        {
            return !string.IsNullOrWhiteSpace(userId) && _connectionCounts.ContainsKey(userId);
        }

        public IReadOnlyCollection<string> GetOnlineUserIds()
        {
            return _connectionCounts.Keys.ToList();
        }

        public bool OnConnected(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var count = _connectionCounts.AddOrUpdate(userId, 1, (_, current) => current + 1);
            return count == 1;
        }

        public bool OnDisconnected(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            while (true)
            {
                if (!_connectionCounts.TryGetValue(userId, out var count))
                {
                    return false;
                }

                if (count <= 1)
                {
                    if (_connectionCounts.TryRemove(userId, out _))
                    {
                        return true;
                    }

                    continue;
                }

                if (_connectionCounts.TryUpdate(userId, count - 1, count))
                {
                    return false;
                }
            }
        }
    }
}
