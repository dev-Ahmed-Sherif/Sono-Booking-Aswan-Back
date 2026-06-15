using SonoBooking.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.RequestAttach
{
    [ExcludeFromCodeCoverage]
    public class EditRequestAttachDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string RequestId { get; set; }
        public string AttachmentId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
