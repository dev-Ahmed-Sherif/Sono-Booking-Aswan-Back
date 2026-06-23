using SonoBooking.Common.Constants.BusinessNotification;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.BusinessNotification.Chat
{
    internal sealed record RequestChatContext(
        string RequestId,
        string RequestNumber,
        string OwnerUserId,
        string? LeaderUserId,
        string? ReceptionUserId);

    internal sealed class RequestChatParticipantResolver(SonoBookingDbContext context)
    {
        public async Task<RequestChatContext?> LoadContextAsync(
            string requestId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(requestId))
            {
                return null;
            }

            var request = await context.Requests
                .AsNoTracking()
                .Include(r => r.Reservation)
                .FirstOrDefaultAsync(r => r.Id == requestId && !r.IsDeleted, cancellationToken);

            if (request == null)
            {
                return null;
            }

            var leaderUserId = await ResolveLeaderUserIdAsync(request.ApprovedById, cancellationToken);
            var receptionUserId = await ResolveReceptionUserIdAsync(
                request.Reservation?.CreatedById,
                cancellationToken);

            return new RequestChatContext(
                request.Id,
                request.RequestNumber,
                request.UserId,
                leaderUserId,
                receptionUserId);
        }

        public IReadOnlyList<string> GetParticipantsForGroupType(RequestChatContext chatContext, string groupType)
        {
            if (!RequestChatGroupTypes.IsValid(groupType))
            {
                return Array.Empty<string>();
            }

            return groupType switch
            {
                RequestChatGroupTypes.OwnerLeader =>
                    ToDistinctParticipants(chatContext.OwnerUserId, chatContext.LeaderUserId),
                RequestChatGroupTypes.LeaderReception =>
                    ToDistinctParticipants(chatContext.LeaderUserId, chatContext.ReceptionUserId),
                RequestChatGroupTypes.OwnerReception =>
                    ToDistinctParticipants(chatContext.OwnerUserId, chatContext.ReceptionUserId),
                _ => Array.Empty<string>()
            };
        }

        public IReadOnlyList<string> GetAvailableGroupTypesForUser(RequestChatContext chatContext, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Array.Empty<string>();
            }

            return RequestChatGroupTypes.All
                .Where(groupType =>
                {
                    var participants = GetParticipantsForGroupType(chatContext, groupType);
                    return participants.Count >= 2 &&
                           participants.Contains(userId, StringComparer.Ordinal);
                })
                .ToList();
        }

        private async Task<string?> ResolveLeaderUserIdAsync(
            string? approvedById,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(approvedById) &&
                await UserHasRoleAsync(approvedById, RoleNames.Leader, cancellationToken))
            {
                return approvedById.Trim();
            }

            return await GetFirstActiveUserIdInRoleAsync(RoleNames.Leader, cancellationToken);
        }

        private async Task<string?> ResolveReceptionUserIdAsync(
            string? reservationCreatedById,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(reservationCreatedById) &&
                await UserHasRoleAsync(reservationCreatedById, RoleNames.ReceptionStaff, cancellationToken))
            {
                return reservationCreatedById.Trim();
            }

            return await GetFirstActiveUserIdInRoleAsync(RoleNames.ReceptionStaff, cancellationToken);
        }

        private async Task<bool> UserHasRoleAsync(
            string userId,
            string roleName,
            CancellationToken cancellationToken)
        {
            return await (
                from userRole in context.UserRoles
                join role in context.Roles on userRole.RoleId equals role.Id
                join user in context.Users on userRole.UserId equals user.Id
                where userRole.UserId == userId &&
                      role.Name == roleName &&
                      !user.IsDeleted
                select userRole.UserId
            ).AnyAsync(cancellationToken);
        }

        private async Task<string?> GetFirstActiveUserIdInRoleAsync(
            string roleName,
            CancellationToken cancellationToken)
        {
            return await (
                from userRole in context.UserRoles
                join role in context.Roles on userRole.RoleId equals role.Id
                join user in context.Users on userRole.UserId equals user.Id
                where role.Name == roleName && !user.IsDeleted
                orderby user.CreatedAt
                select user.Id
            ).FirstOrDefaultAsync(cancellationToken);
        }

        private static IReadOnlyList<string> ToDistinctParticipants(params string?[] userIds) =>
            userIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id!.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToList();
    }
}
