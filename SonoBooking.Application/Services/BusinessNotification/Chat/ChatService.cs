using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SonoBooking.Common.Constants.Auth;
using SonoBooking.Common.Constants.BusinessNotification;
using SonoBooking.Common.Core;
using SonoBooking.Common.DTO.BusinessNotification.Chat;
using SonoBooking.Common.DTO.Identity.User;
using SonoBooking.Domain;
using SonoBooking.Domain.Entities.BusinessNotification;
using SonoBooking.Domain.Entities.Identity;
using SonoBooking.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SonoBooking.Application.Services.BusinessNotification.Chat
{
    public class ChatService(
        SonoBookingDbContext context,
        UserDataDto currentUser,
        IHttpContextAccessor httpContextAccessor,
        IChatRealtimePublisher realtimePublisher) : IChatService
    {
        private const int DefaultMessagePageSize = 50;
        private const int MaxMessagePageSize = 200;
        private readonly RequestChatParticipantResolver _requestChatResolver = new(context);

        public async Task<IFinalResult> GetConversationsAsync(CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            var groups = await context.Set<MessagingGroup>()
                .AsNoTracking()
                .Where(g => !g.IsDeleted && g.Code.StartsWith("chat:"))
                .ToListAsync(cancellationToken);

            var participantGroups = new List<MessagingGroup>();
            foreach (var group in groups)
            {
                if (await IsUserParticipantAsync(group.Code, currentUser.Id, cancellationToken))
                {
                    participantGroups.Add(group);
                }
            }

            if (participantGroups.Count == 0)
            {
                return response.PostResult(Array.Empty<ChatConversationDto>(), HttpStatusCode.OK, message: MessagesConstants.Success);
            }

            if (ChatContactVisibilityResolver.ExcludesGroupConversations(currentUser.Role))
            {
                participantGroups = participantGroups
                    .Where(g => ChatConversationHelper.IsDirectOneToOneConversation(g.Code))
                    .ToList();

                if (participantGroups.Count == 0)
                {
                    return response.PostResult(Array.Empty<ChatConversationDto>(), HttpStatusCode.OK, message: MessagesConstants.Success);
                }
            }

            var groupIds = participantGroups.Select(g => g.Id!).ToList();
            var messages = await context.Messages
                .AsNoTracking()
                .Where(m => !m.IsDeleted && m.MessagingGroupId != null && groupIds.Contains(m.MessagingGroupId))
                .Select(m => new
                {
                    m.MessagingGroupId,
                    m.Content,
                    m.CreatedAt,
                    m.SenderId,
                    m.ReceiverId,
                    m.IsRead
                })
                .ToListAsync(cancellationToken);

            var requestContextCache = new Dictionary<string, RequestChatContext?>(StringComparer.Ordinal);
            var participantIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var group in participantGroups)
            {
                var participants = await ResolveParticipantIdsAsync(
                    group.Code,
                    requestContextCache,
                    cancellationToken);

                foreach (var participantId in participants)
                {
                    participantIds.Add(participantId);
                }
            }

            var users = await context.Users
                .AsNoTracking()
                .Where(u => participantIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

            var conversations = new List<ChatConversationDto>();
            foreach (var group in participantGroups)
            {
                var participants = await ResolveParticipantIdsAsync(
                    group.Code,
                    requestContextCache,
                    cancellationToken);

                var groupMessages = messages
                    .Where(m => m.MessagingGroupId == group.Id)
                    .OrderByDescending(m => m.CreatedAt)
                    .ToList();

                var lastMessage = groupMessages.FirstOrDefault();
                var unreadCount = groupMessages.Count(m =>
                    m.ReceiverId == currentUser.Id && !m.IsRead);

                var otherNames = participants
                    .Where(id => !string.Equals(id, currentUser.Id, StringComparison.Ordinal))
                    .Select(id => users.TryGetValue(id, out var name) ? name : id)
                    .ToList();

                ChatConversationHelper.TryParseRequestConversation(group.Code, out var requestId, out var groupType);
                var displayNames = RequestChatGroupTypes.GetDisplayNames(groupType);
                var requestNumber = requestContextCache.TryGetValue(requestId, out var ctx) && ctx != null
                    ? ctx.RequestNumber
                    : null;

                conversations.Add(new ChatConversationDto
                {
                    Id = group.Id!,
                    Title = ChatConversationHelper.IsRequestConversation(group.Code)
                        ? BuildRequestConversationTitle(displayNames.NameAr, requestNumber, otherNames)
                        : otherNames.Count > 0 ? string.Join(", ", otherNames) : group.NameAr,
                    LastMessagePreview = lastMessage?.Content,
                    UpdatedAt = lastMessage?.CreatedAt ?? group.ModifiedAt,
                    ParticipantNames = participants
                        .Select(id => users.TryGetValue(id, out var name) ? name : id)
                        .ToList(),
                    ParticipantUserIds = participants.ToList(),
                    UnreadCount = unreadCount,
                    GroupType = ChatConversationHelper.IsRequestConversation(group.Code) ? groupType : null,
                    RequestId = ChatConversationHelper.IsRequestConversation(group.Code) ? requestId : null,
                    RequestNumber = requestNumber
                });
            }

            conversations = conversations
                .OrderByDescending(c => c.UpdatedAt)
                .ToList();

            return response.PostResult(conversations, HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public async Task<IFinalResult> GetChatContactsAsync(CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            var allowedRoles = ChatContactVisibilityResolver
                .GetAllowedTargetRoles(currentUser.Role)
                .ToList();

            if (allowedRoles.Count == 0 &&
                !ChatContactVisibilityResolver.AllowsHistoryPartners(currentUser.Role))
            {
                return response.PostResult(Array.Empty<ChatContactDto>(), HttpStatusCode.OK, message: MessagesConstants.Success);
            }

            var contacts = allowedRoles.Count == 0
                ? new List<ChatContactDto>()
                : await (
                    from user in context.Users.AsNoTracking()
                    join userRole in context.UserRoles on user.Id equals userRole.UserId
                    join role in context.Roles on userRole.RoleId equals role.Id
                    where !user.IsDeleted &&
                          user.Id != currentUser.Id &&
                          allowedRoles.Contains(role.Name!)
                    orderby user.FullName
                    select new ChatContactDto
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Role = role.Name!
                    })
                    .ToListAsync(cancellationToken);

            if (ChatContactVisibilityResolver.AllowsHistoryPartners(currentUser.Role))
            {
                var historyPartnerIds = await GetConversationPartnerUserIdsAsync(
                    currentUser.Id,
                    directOneToOneOnly: ChatContactVisibilityResolver.RestrictsHistoryToDirectUserContacts(currentUser.Role),
                    cancellationToken);

                var existingContactIds = contacts
                    .Select(c => c.UserId)
                    .ToHashSet(StringComparer.Ordinal);

                var additionalPartnerIds = historyPartnerIds
                    .Where(id => !existingContactIds.Contains(id))
                    .ToList();

                if (additionalPartnerIds.Count > 0)
                {
                    var historyContacts = await (
                        from user in context.Users.AsNoTracking()
                        join userRole in context.UserRoles on user.Id equals userRole.UserId
                        join role in context.Roles on userRole.RoleId equals role.Id
                        where !user.IsDeleted &&
                              additionalPartnerIds.Contains(user.Id)
                        orderby user.FullName
                        select new ChatContactDto
                        {
                            UserId = user.Id,
                            FullName = user.FullName,
                            Role = role.Name!
                        })
                        .ToListAsync(cancellationToken);

                    if (ChatContactVisibilityResolver.RestrictsHistoryToDirectUserContacts(currentUser.Role))
                    {
                        historyContacts = historyContacts
                            .Where(c => string.Equals(c.Role, RoleNames.User, StringComparison.Ordinal))
                            .ToList();
                    }

                    contacts.AddRange(historyContacts);
                }
            }

            contacts = contacts
                .GroupBy(c => c.UserId, StringComparer.Ordinal)
                .Select(g => g.First())
                .OrderBy(c => c.FullName, StringComparer.Ordinal)
                .ToList();

            return response.PostResult(contacts, HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public async Task<IFinalResult> GetMessagesAsync(
            string conversationId,
            int take = DefaultMessagePageSize,
            string? beforeMessageId = null,
            CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            if (string.IsNullOrWhiteSpace(conversationId))
            {
                return response.PostResult(null, HttpStatusCode.BadRequest, message: "conversationId is required.");
            }

            take = Math.Clamp(take, 1, MaxMessagePageSize);

            var group = await context.Set<MessagingGroup>()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == conversationId && !g.IsDeleted, cancellationToken);

            if (group == null || !await IsUserParticipantAsync(group.Code, currentUser.Id, cancellationToken))
            {
                return response.PostResult(null, HttpStatusCode.NotFound, message: "Conversation not found.");
            }

            DateTime? beforeCreatedAt = null;
            if (!string.IsNullOrWhiteSpace(beforeMessageId))
            {
                beforeCreatedAt = await context.Messages
                    .AsNoTracking()
                    .Where(m => m.Id == beforeMessageId && m.MessagingGroupId == conversationId)
                    .Select(m => (DateTime?)m.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var query = context.Messages
                .AsNoTracking()
                .Where(m =>
                    !m.IsDeleted &&
                    m.MessagingGroupId == conversationId &&
                    (!beforeCreatedAt.HasValue || m.CreatedAt < beforeCreatedAt.Value));

            var page = await query
                .OrderByDescending(m => m.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);

            var senderIds = page.Select(m => m.SenderId).Distinct().ToList();
            var senderNames = await context.Users
                .AsNoTracking()
                .Where(u => senderIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

            var unreadForCurrentUser = await context.Messages
                .Where(m =>
                    !m.IsDeleted &&
                    m.MessagingGroupId == conversationId &&
                    m.ReceiverId == currentUser.Id &&
                    !m.IsRead)
                .ToListAsync(cancellationToken);

            if (unreadForCurrentUser.Count > 0)
            {
                var now = DateTime.UtcNow;
                foreach (var message in unreadForCurrentUser)
                {
                    message.IsRead = true;
                    message.ModifiedAt = now;
                    message.ModifiedById = currentUser.Id;
                    message.ModifiedBy = currentUser.Name;
                }

                await context.SaveChangesAsync(cancellationToken);

                await realtimePublisher.PublishConversationUpdatedAsync(
                    currentUser.Id,
                    new ChatConversationUpdatedDto
                    {
                        Id = conversationId,
                        UnreadCount = 0
                    },
                    cancellationToken);

                await PublishChatUnreadCountAsync(currentUser.Id, cancellationToken);
            }

            var dtos = page
                .OrderBy(m => m.CreatedAt)
                .Select(m => ToMessageDto(m, conversationId, senderNames))
                .ToList();

            return response.PostResult(dtos, HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public async Task<IFinalResult> CreateConversationAsync(
            IReadOnlyList<string> participantUserIds,
            CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            if (participantUserIds == null || participantUserIds.Count == 0)
            {
                return response.PostResult(null, HttpStatusCode.BadRequest, message: "At least one participant is required.");
            }

            var participantSet = participantUserIds
                .Append(currentUser.Id)
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToList();

            if (participantSet.Count < 2)
            {
                return response.PostResult(null, HttpStatusCode.BadRequest, message: "A conversation requires at least two participants.");
            }

            var existingUsers = await context.Users
                .AsNoTracking()
                .Where(u => participantSet.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToListAsync(cancellationToken);

            if (existingUsers.Count != participantSet.Count)
            {
                return response.PostResult(null, HttpStatusCode.BadRequest, message: "One or more participants were not found.");
            }

            var participantRoles = await GetUserRolesByUserIdsAsync(participantSet, cancellationToken);
            var historyPartnerIds = ChatContactVisibilityResolver.AllowsHistoryPartners(currentUser.Role)
                ? await GetConversationPartnerUserIdsAsync(
                    currentUser.Id,
                    directOneToOneOnly: ChatContactVisibilityResolver.RestrictsHistoryToDirectUserContacts(currentUser.Role),
                    cancellationToken)
                : null;

            foreach (var participantId in participantSet.Where(id =>
                         !string.Equals(id, currentUser.Id, StringComparison.Ordinal)))
            {
                participantRoles.TryGetValue(participantId, out var targetRole);
                var isHistoryPartner = historyPartnerIds?.Contains(participantId) == true;
                if (!ChatContactVisibilityResolver.CanChatWith(currentUser.Role, targetRole, isHistoryPartner))
                {
                    return response.PostResult(
                        null,
                        HttpStatusCode.Forbidden,
                        message: "You cannot start a conversation with one or more participants.");
                }
            }

            var conversationCode = ChatConversationHelper.BuildConversationCode(participantSet);
            var existingGroup = await context.Set<MessagingGroup>()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Code == conversationCode && !g.IsDeleted, cancellationToken);

            if (existingGroup != null)
            {
                return response.PostResult(
                    await BuildConversationDtoAsync(existingGroup, cancellationToken),
                    HttpStatusCode.OK,
                    message: MessagesConstants.Success);
            }

            var now = DateTime.UtcNow;
            var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var otherNames = existingUsers
                .Where(u => !string.Equals(u.Id, currentUser.Id, StringComparison.Ordinal))
                .Select(u => u.FullName)
                .ToList();

            var group = new MessagingGroup
            {
                Code = conversationCode,
                NameAr = otherNames.Count > 0 ? string.Join(", ", otherNames) : "محادثة",
                NameEn = otherNames.Count > 0 ? string.Join(", ", otherNames) : "Conversation",
                GovernorateId = string.IsNullOrWhiteSpace(currentUser.GovernorateId) ? null : currentUser.GovernorateId,
                CreatedAt = now,
                CreatedById = currentUser.Id,
                CreatedBy = currentUser.Name,
                ModifiedAt = now,
                ModifiedById = currentUser.Id,
                ModifiedBy = currentUser.Name,
                IpAddress = ip
            };

            context.Set<MessagingGroup>().Add(group);
            await context.SaveChangesAsync(cancellationToken);

            return response.PostResult(
                await BuildConversationDtoAsync(group, cancellationToken),
                HttpStatusCode.Created,
                message: MessagesConstants.Success);
        }

        public async Task<IFinalResult> SendMessageAsync(
            string conversationId,
            string content,
            bool publishRealtime = true,
            CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            if (string.IsNullOrWhiteSpace(conversationId) || string.IsNullOrWhiteSpace(content))
            {
                return response.PostResult(null, HttpStatusCode.BadRequest, message: "conversationId and content are required.");
            }

            var trimmedContent = content.Trim();
            if (trimmedContent.Length == 0)
            {
                return response.PostResult(null, HttpStatusCode.BadRequest, message: "content is required.");
            }

            var group = await context.Set<MessagingGroup>()
                .FirstOrDefaultAsync(g => g.Id == conversationId && !g.IsDeleted, cancellationToken);

            if (group == null || !await IsUserParticipantAsync(group.Code, currentUser.Id, cancellationToken))
            {
                return response.PostResult(null, HttpStatusCode.NotFound, message: "Conversation not found.");
            }

            var participants = await ResolveParticipantIdsAsync(group.Code, cancellationToken: cancellationToken);
            var receiverId = participants.FirstOrDefault(id =>
                !string.Equals(id, currentUser.Id, StringComparison.Ordinal));

            if (string.IsNullOrWhiteSpace(receiverId))
            {
                return response.PostResult(null, HttpStatusCode.BadRequest, message: "Conversation has no recipient.");
            }

            var now = DateTime.UtcNow;
            var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var message = new Message
            {
                Content = trimmedContent,
                SenderId = currentUser.Id,
                ReceiverId = receiverId,
                MessagingGroupId = conversationId,
                GovernorateId = string.IsNullOrWhiteSpace(currentUser.GovernorateId) ? null : currentUser.GovernorateId,
                IsRead = false,
                CreatedAt = now,
                CreatedById = currentUser.Id,
                CreatedBy = currentUser.Name,
                ModifiedAt = now,
                ModifiedById = currentUser.Id,
                ModifiedBy = currentUser.Name,
                IpAddress = ip
            };

            context.Messages.Add(message);
            group.ModifiedAt = now;
            group.ModifiedById = currentUser.Id;
            group.ModifiedBy = currentUser.Name;
            await context.SaveChangesAsync(cancellationToken);

            var dto = ToMessageDto(message, conversationId, new Dictionary<string, string>
            {
                [currentUser.Id] = currentUser.Name
            });

            if (publishRealtime)
            {
                await realtimePublisher.PublishMessageAsync(dto, participants, cancellationToken);

                foreach (var participantId in participants)
                {
                    var unreadCount = await GetConversationUnreadCountAsync(
                        conversationId,
                        participantId,
                        cancellationToken);

                    await realtimePublisher.PublishConversationUpdatedAsync(
                        participantId,
                        new ChatConversationUpdatedDto
                        {
                            Id = conversationId,
                            LastMessagePreview = trimmedContent,
                            UpdatedAt = now,
                            UnreadCount = unreadCount
                        },
                        cancellationToken);

                    await PublishChatUnreadCountAsync(participantId, cancellationToken);
                }
            }

            return response.PostResult(dto, HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        public async Task<bool> EnsureParticipantAsync(
            string conversationId,
            string userId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(conversationId) || string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var code = await context.Set<MessagingGroup>()
                .AsNoTracking()
                .Where(g => g.Id == conversationId && !g.IsDeleted)
                .Select(g => g.Code)
                .FirstOrDefaultAsync(cancellationToken);

            return await IsUserParticipantAsync(code, userId, cancellationToken);
        }

        public async Task<IFinalResult> GetOrCreateRequestConversationAsync(
            string requestId,
            string groupType,
            CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            if (string.IsNullOrWhiteSpace(requestId) || !RequestChatGroupTypes.IsValid(groupType))
            {
                return response.PostResult(null, HttpStatusCode.BadRequest, message: "requestId and a valid groupType are required.");
            }

            var chatContext = await _requestChatResolver.LoadContextAsync(requestId.Trim(), cancellationToken);
            if (chatContext == null)
            {
                return response.PostResult(null, HttpStatusCode.NotFound, message: "Request not found.");
            }

            var participants = _requestChatResolver.GetParticipantsForGroupType(chatContext, groupType.Trim());
            if (participants.Count < 2)
            {
                return response.PostResult(
                    null,
                    HttpStatusCode.BadRequest,
                    message: "Could not resolve both participants for this conversation.");
            }

            if (!participants.Contains(currentUser.Id, StringComparer.Ordinal))
            {
                return response.PostResult(null, HttpStatusCode.Forbidden, message: "You are not a participant in this conversation.");
            }

            var conversationCode = ChatConversationHelper.BuildRequestConversationCode(chatContext.RequestId, groupType.Trim());
            var existingGroup = await context.Set<MessagingGroup>()
                .FirstOrDefaultAsync(g => g.Code == conversationCode && !g.IsDeleted, cancellationToken);

            if (existingGroup != null)
            {
                return response.PostResult(
                    await BuildConversationDtoAsync(existingGroup, chatContext, groupType.Trim(), cancellationToken),
                    HttpStatusCode.OK,
                    message: MessagesConstants.Success);
            }

            var displayNames = RequestChatGroupTypes.GetDisplayNames(groupType.Trim());
            var now = DateTime.UtcNow;
            var ip = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var group = new MessagingGroup
            {
                Code = conversationCode,
                NameAr = $"{displayNames.NameAr} — {chatContext.RequestNumber}",
                NameEn = $"{displayNames.NameEn} — {chatContext.RequestNumber}",
                GovernorateId = string.IsNullOrWhiteSpace(currentUser.GovernorateId) ? null : currentUser.GovernorateId,
                CreatedAt = now,
                CreatedById = currentUser.Id,
                CreatedBy = currentUser.Name,
                ModifiedAt = now,
                ModifiedById = currentUser.Id,
                ModifiedBy = currentUser.Name,
                IpAddress = ip
            };

            context.Set<MessagingGroup>().Add(group);
            await context.SaveChangesAsync(cancellationToken);

            return response.PostResult(
                await BuildConversationDtoAsync(group, chatContext, groupType.Trim(), cancellationToken),
                HttpStatusCode.Created,
                message: MessagesConstants.Success);
        }

        public async Task<IFinalResult> GetRequestConversationsAsync(
            string requestId,
            CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();
            if (string.IsNullOrWhiteSpace(currentUser.Id))
            {
                return response.PostResult(null, HttpStatusCode.Unauthorized, message: "Unauthorized.");
            }

            if (string.IsNullOrWhiteSpace(requestId))
            {
                return response.PostResult(null, HttpStatusCode.BadRequest, message: "requestId is required.");
            }

            var chatContext = await _requestChatResolver.LoadContextAsync(requestId.Trim(), cancellationToken);
            if (chatContext == null)
            {
                return response.PostResult(null, HttpStatusCode.NotFound, message: "Request not found.");
            }

            var availableGroupTypes = _requestChatResolver.GetAvailableGroupTypesForUser(chatContext, currentUser.Id);
            if (availableGroupTypes.Count == 0)
            {
                return response.PostResult(Array.Empty<ChatConversationDto>(), HttpStatusCode.OK, message: MessagesConstants.Success);
            }

            var conversations = new List<ChatConversationDto>();
            foreach (var groupType in availableGroupTypes)
            {
                var result = await GetOrCreateRequestConversationAsync(chatContext.RequestId, groupType, cancellationToken);
                if (result.Data is ChatConversationDto dto)
                {
                    conversations.Add(dto);
                }
            }

            return response.PostResult(conversations, HttpStatusCode.OK, message: MessagesConstants.Success);
        }

        private async Task<bool> IsUserParticipantAsync(
            string? code,
            string userId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(userId))
            {
                return false;
            }

            var participants = await ResolveParticipantIdsAsync(code, cancellationToken: cancellationToken);
            return participants.Contains(userId, StringComparer.Ordinal);
        }

        private async Task<IReadOnlyList<string>> ResolveParticipantIdsAsync(
            string? code,
            IDictionary<string, RequestChatContext?>? requestContextCache = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Array.Empty<string>();
            }

            if (!ChatConversationHelper.TryParseRequestConversation(code, out var requestId, out var groupType))
            {
                return ChatConversationHelper.ParseParticipantIds(code);
            }

            RequestChatContext? chatContext = null;
            if (requestContextCache != null &&
                requestContextCache.TryGetValue(requestId, out chatContext))
            {
                return chatContext == null
                    ? Array.Empty<string>()
                    : _requestChatResolver.GetParticipantsForGroupType(chatContext, groupType);
            }

            chatContext = await _requestChatResolver.LoadContextAsync(requestId, cancellationToken);
            requestContextCache?.TryAdd(requestId, chatContext);

            return chatContext == null
                ? Array.Empty<string>()
                : _requestChatResolver.GetParticipantsForGroupType(chatContext, groupType);
        }

        private async Task<HashSet<string>> GetConversationPartnerUserIdsAsync(
            string userId,
            bool directOneToOneOnly = false,
            CancellationToken cancellationToken = default)
        {
            var partnerIds = new HashSet<string>(StringComparer.Ordinal);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return partnerIds;
            }

            var groups = await context.Set<MessagingGroup>()
                .AsNoTracking()
                .Where(g => !g.IsDeleted && g.Code.StartsWith("chat:"))
                .ToListAsync(cancellationToken);

            var requestContextCache = new Dictionary<string, RequestChatContext?>(StringComparer.Ordinal);
            foreach (var group in groups)
            {
                if (directOneToOneOnly && !ChatConversationHelper.IsDirectOneToOneConversation(group.Code))
                {
                    continue;
                }

                var participants = await ResolveParticipantIdsAsync(
                    group.Code,
                    requestContextCache,
                    cancellationToken);

                if (!participants.Contains(userId, StringComparer.Ordinal))
                {
                    continue;
                }

                foreach (var participantId in participants)
                {
                    if (!string.Equals(participantId, userId, StringComparison.Ordinal))
                    {
                        partnerIds.Add(participantId);
                    }
                }
            }

            return partnerIds;
        }

        private async Task<Dictionary<string, string>> GetUserRolesByUserIdsAsync(
            IReadOnlyCollection<string> userIds,
            CancellationToken cancellationToken)
        {
            if (userIds.Count == 0)
            {
                return new Dictionary<string, string>(StringComparer.Ordinal);
            }

            var rows = await (
                from userRole in context.UserRoles.AsNoTracking()
                join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                where userIds.Contains(userRole.UserId)
                select new { userRole.UserId, RoleName = role.Name! }
            ).ToListAsync(cancellationToken);

            return rows
                .GroupBy(r => r.UserId, StringComparer.Ordinal)
                .ToDictionary(g => g.Key, g => g.First().RoleName, StringComparer.Ordinal);
        }

        private static string BuildRequestConversationTitle(
            string groupLabel,
            string? requestNumber,
            IReadOnlyList<string> otherNames)
        {
            var requestLabel = string.IsNullOrWhiteSpace(requestNumber) ? string.Empty : $" ({requestNumber})";
            if (otherNames.Count == 0)
            {
                return $"{groupLabel}{requestLabel}";
            }

            return $"{groupLabel}{requestLabel} — {string.Join(", ", otherNames)}";
        }

        private async Task<ChatConversationDto> BuildConversationDtoAsync(
            MessagingGroup group,
            CancellationToken cancellationToken)
        {
            RequestChatContext? chatContext = null;
            string? groupType = null;
            if (ChatConversationHelper.TryParseRequestConversation(group.Code, out var requestId, out var parsedGroupType))
            {
                groupType = parsedGroupType;
                chatContext = await _requestChatResolver.LoadContextAsync(requestId, cancellationToken);
            }

            return await BuildConversationDtoAsync(group, chatContext, groupType, cancellationToken);
        }

        private async Task<ChatConversationDto> BuildConversationDtoAsync(
            MessagingGroup group,
            RequestChatContext? chatContext,
            string? groupType,
            CancellationToken cancellationToken)
        {
            var participants = await ResolveParticipantIdsAsync(group.Code, cancellationToken: cancellationToken);
            var users = await context.Users
                .AsNoTracking()
                .Where(u => participants.Contains(u.Id))
                .Select(u => new { u.Id, u.FullName })
                .ToDictionaryAsync(u => u.Id, u => u.FullName, cancellationToken);

            var otherNames = participants
                .Where(id => !string.Equals(id, currentUser.Id, StringComparison.Ordinal))
                .Select(id => users.TryGetValue(id, out var name) ? name : id)
                .ToList();

            var displayNames = RequestChatGroupTypes.GetDisplayNames(groupType ?? string.Empty);
            var isRequestConversation = ChatConversationHelper.IsRequestConversation(group.Code);

            return new ChatConversationDto
            {
                Id = group.Id!,
                Title = isRequestConversation
                    ? BuildRequestConversationTitle(displayNames.NameAr, chatContext?.RequestNumber, otherNames)
                    : otherNames.Count > 0 ? string.Join(", ", otherNames) : group.NameAr,
                ParticipantNames = participants
                    .Select(id => users.TryGetValue(id, out var name) ? name : id)
                    .ToList(),
                ParticipantUserIds = participants.ToList(),
                UpdatedAt = group.ModifiedAt,
                UnreadCount = 0,
                GroupType = isRequestConversation ? groupType : null,
                RequestId = chatContext?.RequestId,
                RequestNumber = chatContext?.RequestNumber
            };
        }

        private static ChatMessageDto ToMessageDto(
            Message message,
            string conversationId,
            IReadOnlyDictionary<string, string> senderNames)
        {
            senderNames.TryGetValue(message.SenderId, out var senderName);

            return new ChatMessageDto
            {
                Id = message.Id!,
                ConversationId = conversationId,
                SenderId = message.SenderId,
                SenderName = senderName,
                Content = message.Content,
                CreatedAt = message.CreatedAt
            };
        }

        private async Task<int> GetConversationUnreadCountAsync(
            string conversationId,
            string userId,
            CancellationToken cancellationToken)
        {
            return await context.Messages
                .AsNoTracking()
                .CountAsync(m =>
                    !m.IsDeleted &&
                    m.MessagingGroupId == conversationId &&
                    m.ReceiverId == userId &&
                    !m.IsRead,
                    cancellationToken);
        }

        private async Task<int> GetTotalUnreadCountForUserAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return 0;
            }

            return await context.Messages
                .AsNoTracking()
                .CountAsync(m =>
                    !m.IsDeleted &&
                    m.ReceiverId == userId &&
                    !m.IsRead,
                    cancellationToken);
        }

        private Task PublishChatUnreadCountAsync(
            string userId,
            CancellationToken cancellationToken)
        {
            return PublishChatUnreadCountAsync(userId, null, cancellationToken);
        }

        private async Task PublishChatUnreadCountAsync(
            string userId,
            int? unreadCount,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var totalUnread = unreadCount
                ?? await GetTotalUnreadCountForUserAsync(userId, cancellationToken);

            await realtimePublisher.PublishChatUnreadCountAsync(
                userId,
                totalUnread,
                cancellationToken);
        }

        public async Task<IFinalResult> GetOnlineStatusesAsync(
            IReadOnlyList<string> userIds,
            CancellationToken cancellationToken = default)
        {
            var response = new ResponseResult();

            if (userIds == null || userIds.Count == 0)
            {
                return response.PostResult(
                    Array.Empty<UserOnlineStatusDto>(),
                    HttpStatusCode.OK,
                    message: HttpStatusCode.OK.ToString());
            }

            var normalizedUserIds = userIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (normalizedUserIds.Length == 0)
            {
                return response.PostResult(
                    Array.Empty<UserOnlineStatusDto>(),
                    HttpStatusCode.OK,
                    message: HttpStatusCode.OK.ToString());
            }

            var normalizedLowerSet = normalizedUserIds
                .Select(id => id.ToLowerInvariant())
                .ToHashSet();

            var loggedInUsers = await context.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted && u.IsLogedIn && normalizedLowerSet.Contains(u.Id.ToLower()))
                .Select(u => u.Id)
                .ToListAsync(cancellationToken);

            var loggedInSet = new HashSet<string>(loggedInUsers, StringComparer.OrdinalIgnoreCase);

            var statuses = normalizedUserIds
                .Select(id => new UserOnlineStatusDto
                {
                    UserId = id,
                    IsOnline = loggedInSet.Contains(id)
                })
                .ToArray();

            return response.PostResult(
                statuses,
                HttpStatusCode.OK,
                message: HttpStatusCode.OK.ToString());
        }
    }
}
