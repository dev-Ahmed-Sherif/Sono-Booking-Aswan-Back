using SonoTracker.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Lookup.Attach
{
    [ExcludeFromCodeCoverage]
    public class AttachDto : IEntityDto<string>
    {
        public string Id { get; set; }

        public string FileName { get; set; }

        public string Extension { get; set; }

        public string Path { get; set; }
    }
}
