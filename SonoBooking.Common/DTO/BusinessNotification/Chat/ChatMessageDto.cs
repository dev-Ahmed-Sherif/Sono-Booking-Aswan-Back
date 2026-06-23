using System;

namespace SonoBooking.Common.DTO.BusinessNotification.Chat
{
    public class ChatMessageDto
    {
        public required string Id { get; set; }
        public required string ConversationId { get; set; }
        public required string SenderId { get; set; }
        public string? SenderName { get; set; }
        public required string Content { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
