using SonoBooking.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.RequestAttach
{
    [ExcludeFromCodeCoverage]
    public class RequestAttachDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string RequestId { get; set; }
        public string AttachmentId { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string Url { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsDeleted { get; set; }
    }
}
