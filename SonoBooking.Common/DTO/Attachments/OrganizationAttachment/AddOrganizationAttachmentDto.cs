using SonoBooking.Common.Core;

namespace SonoBooking.Common.DTO.Attachments.OrganizationAttachment
{
    public class AddOrganizationAttachmentDto : IEntityDto<string>
    {
        public string? Id { get; set; }
        public string FileId { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public string Url { get; set; } = string.Empty;
        public string AttachmentDisplaySize { get; set; } = string.Empty;
        public string OrganizationId { get; set; } = string.Empty;
    }
}


