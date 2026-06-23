namespace SonoBooking.Common.DTO.BusinessNotification.Chat
{
    public class UserOnlineStatusDto
    {
        public string UserId { get; set; } = string.Empty;

        public bool IsOnline { get; set; }
    }
}
