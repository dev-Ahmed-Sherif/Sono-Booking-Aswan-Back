using Microsoft.AspNetCore.Http;
using SonoBooking.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.RequestAttach
{
    [ExcludeFromCodeCoverage]
    public class AddRequestAttachDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string RequestId { get; set; }
        public string AttachmentId { get; set; }
        public bool IsPrimary { get; set; }
        public IFormFile Image { get; set; }
    }
}
