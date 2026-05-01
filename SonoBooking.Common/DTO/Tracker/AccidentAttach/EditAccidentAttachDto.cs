using SonoTracker.Common.Core;

namespace SonoTracker.Common.DTO.Tracker.AccidentAttach
{
    public class EditAccidentAttachDto : IEntityDto<string>
    {
        public string Id { get; set; }

        public string AttachId { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public string AccidentId { get; set; }
    }
}
