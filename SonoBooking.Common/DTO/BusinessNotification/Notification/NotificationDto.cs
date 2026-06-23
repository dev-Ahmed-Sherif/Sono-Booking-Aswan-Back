using System;

namespace SonoBooking.Common.DTO.BusinessNotification.Notification
{
    public class NotificationDto
    {
        public required string Id { get; set; }
        public required string Type { get; set; }
        public required string Content { get; set; }
        public string? ReferenceId { get; set; }
        public string? GroupType { get; set; }
        public string? GroupNameAr { get; set; }
        public string? GroupNameEn { get; set; }
        public required string SenderId { get; set; }
        public string? SenderName { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
