using System;

namespace SonoBooking.Common.DTO.BusinessNotification.Chat
{
    public class ChatConversationUpdatedDto
    {
        public required string Id { get; set; }
        public string? LastMessagePreview { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int UnreadCount { get; set; }
    }
}
