using System;
using System.Collections.Generic;

namespace SonoBooking.Common.DTO.BusinessNotification.Chat
{
    public class ChatConversationDto
    {
        public required string Id { get; set; }
        public string? Title { get; set; }
        public string? LastMessagePreview { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<string>? ParticipantNames { get; set; }
        public List<string>? ParticipantUserIds { get; set; }
        public int UnreadCount { get; set; }
        public string? GroupType { get; set; }
        public string? RequestId { get; set; }
        public string? RequestNumber { get; set; }
    }
}
