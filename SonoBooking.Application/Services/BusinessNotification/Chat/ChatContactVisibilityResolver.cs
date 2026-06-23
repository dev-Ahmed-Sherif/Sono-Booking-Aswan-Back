using SonoBooking.Common.Constants.Auth;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SonoBooking.Application.Services.BusinessNotification.Chat
{
    /// <summary>
    /// Role-based rules for who may appear in chat contact lists and direct conversations.
    /// </summary>
    internal static class ChatContactVisibilityResolver
    {
        /// <summary>
        /// User → Leader, ReceptionStaff
        /// Leader → all ReceptionStaff; User contacts only from direct 1:1 history (no groups)
        /// ReceptionStaff → all Leaders; User contacts only from direct 1:1 history (no groups)
        /// Admin / SuperAdmin → User, Leader, ReceptionStaff
        /// </summary>
        private static readonly IReadOnlyDictionary<string, HashSet<string>> AllowedTargetRolesByRole =
            new Dictionary<string, HashSet<string>>(StringComparer.Ordinal)
            {
                [RoleNames.User] = new HashSet<string>(StringComparer.Ordinal)
                {
                    RoleNames.Leader,
                    RoleNames.ReceptionStaff
                },
                [RoleNames.Leader] = new HashSet<string>(StringComparer.Ordinal)
                {
                    RoleNames.ReceptionStaff
                },
                [RoleNames.ReceptionStaff] = new HashSet<string>(StringComparer.Ordinal)
                {
                    RoleNames.Leader
                },
                [RoleNames.Admin] = new HashSet<string>(StringComparer.Ordinal)
                {
                    RoleNames.User,
                    RoleNames.Leader,
                    RoleNames.ReceptionStaff
                },
                [RoleNames.SuperAdmin] = new HashSet<string>(StringComparer.Ordinal)
                {
                    RoleNames.User,
                    RoleNames.Leader,
                    RoleNames.ReceptionStaff
                }
            };

        public static IReadOnlyCollection<string> GetAllowedTargetRoles(string? currentUserRole)
        {
            if (string.IsNullOrWhiteSpace(currentUserRole) ||
                !AllowedTargetRolesByRole.TryGetValue(currentUserRole.Trim(), out var roles))
            {
                return Array.Empty<string>();
            }

            return roles;
        }

        public static bool AllowsHistoryPartners(string? currentUserRole) =>
            string.Equals(currentUserRole?.Trim(), RoleNames.Leader, StringComparison.Ordinal) ||
            string.Equals(currentUserRole?.Trim(), RoleNames.ReceptionStaff, StringComparison.Ordinal);

        public static bool RestrictsHistoryToDirectUserContacts(string? currentUserRole)
        {
            var role = currentUserRole?.Trim();
            return string.Equals(role, RoleNames.Leader, StringComparison.Ordinal) ||
                   string.Equals(role, RoleNames.ReceptionStaff, StringComparison.Ordinal);
        }

        public static bool ExcludesGroupConversations(string? currentUserRole) =>
            RestrictsHistoryToDirectUserContacts(currentUserRole);

        public static bool CanChatWith(string? currentUserRole, string? targetUserRole)
        {
            if (string.IsNullOrWhiteSpace(currentUserRole) || string.IsNullOrWhiteSpace(targetUserRole))
            {
                return false;
            }

            return GetAllowedTargetRoles(currentUserRole)
                .Contains(targetUserRole.Trim(), StringComparer.Ordinal);
        }

        public static bool CanChatWith(
            string? currentUserRole,
            string? targetUserRole,
            bool isHistoryPartner)
        {
            if (isHistoryPartner && AllowsHistoryPartners(currentUserRole))
            {
                if (RestrictsHistoryToDirectUserContacts(currentUserRole))
                {
                    return string.Equals(targetUserRole?.Trim(), RoleNames.User, StringComparison.Ordinal);
                }

                return true;
            }

            return CanChatWith(currentUserRole, targetUserRole);
        }
    }
}
