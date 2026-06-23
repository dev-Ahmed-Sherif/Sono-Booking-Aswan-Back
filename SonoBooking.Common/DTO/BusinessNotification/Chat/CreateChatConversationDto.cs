using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SonoBooking.Common.DTO.BusinessNotification.Chat
{
    public class CreateChatConversationDto
    {
        [Required, MinLength(1)]
        public required List<string> ParticipantUserIds { get; set; }
    }
}
