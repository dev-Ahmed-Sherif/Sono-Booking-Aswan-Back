using System.ComponentModel.DataAnnotations;

namespace SonoBooking.Common.DTO.BusinessNotification.Chat
{
    public class SendChatMessageDto
    {
        [Required]
        public required string ConversationId { get; set; }

        [Required, MinLength(1)]
        public required string Content { get; set; }
    }
}
