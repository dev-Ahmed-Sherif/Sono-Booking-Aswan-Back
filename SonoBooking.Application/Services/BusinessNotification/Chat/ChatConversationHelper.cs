using System;
using System.Collections.Generic;
using System.Linq;

namespace SonoBooking.Application.Services.BusinessNotification.Chat
{
    internal static class ChatConversationHelper
    {
        private const string CodePrefix = "chat:";
        private const string RequestPrefix = "chat:request:";

        public static string BuildConversationCode(IEnumerable<string> participantIds)
        {
            var sorted = participantIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct(StringComparer.Ordinal)
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToList();

            return $"{CodePrefix}{string.Join("|", sorted)}";
        }

        public static string BuildRequestConversationCode(string requestId, string groupType)
        {
            if (string.IsNullOrWhiteSpace(requestId))
            {
                throw new ArgumentException("requestId is required.", nameof(requestId));
            }

            if (string.IsNullOrWhiteSpace(groupType))
            {
                throw new ArgumentException("groupType is required.", nameof(groupType));
            }

            return $"{RequestPrefix}{requestId.Trim()}:{groupType.Trim()}";
        }

        public static IReadOnlyList<string> ParseParticipantIds(string? code)
        {
            if (string.IsNullOrWhiteSpace(code) ||
                !code.StartsWith(CodePrefix, StringComparison.Ordinal) ||
                code.StartsWith(RequestPrefix, StringComparison.Ordinal))
            {
                return Array.Empty<string>();
            }

            return code[CodePrefix.Length..]
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        public static bool TryParseRequestConversation(
            string? code,
            out string requestId,
            out string groupType)
        {
            requestId = string.Empty;
            groupType = string.Empty;

            if (string.IsNullOrWhiteSpace(code) ||
                !code.StartsWith(RequestPrefix, StringComparison.Ordinal))
            {
                return false;
            }

            var remainder = code[RequestPrefix.Length..];
            var separatorIndex = remainder.IndexOf(':');
            if (separatorIndex <= 0 || separatorIndex >= remainder.Length - 1)
            {
                return false;
            }

            requestId = remainder[..separatorIndex].Trim();
            groupType = remainder[(separatorIndex + 1)..].Trim();
            return !string.IsNullOrWhiteSpace(requestId) && !string.IsNullOrWhiteSpace(groupType);
        }

        public static bool IsDirectParticipant(string? code, string userId)
        {
            return ParseParticipantIds(code).Contains(userId, StringComparer.Ordinal);
        }

        public static bool IsRequestConversation(string? code) =>
            !string.IsNullOrWhiteSpace(code) &&
            code.StartsWith(RequestPrefix, StringComparison.Ordinal);

        public static bool IsDirectOneToOneConversation(string? code) =>
            !IsRequestConversation(code) &&
            ParseParticipantIds(code).Count == 2;
    }
}
