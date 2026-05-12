using Microsoft.AspNetCore.Http;
using SonoBooking.Common.Core;

namespace SonoBooking.Common.DTO.Lookup.Attachment
{
    public class AddAttachmentDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string AttachFolder { get; set; }
        public IFormFile File { get; set; }
        public string Url { get; set; }
    }
}


