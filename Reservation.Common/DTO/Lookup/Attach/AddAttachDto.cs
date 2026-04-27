using Microsoft.AspNetCore.Http;
using SonoTracker.Common.Core;
using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Lookup.Attach
{
    [ExcludeFromCodeCoverage]
    public class AddAttachDto : IEntityDto<string>
    {
        public string Id { get; set; }

        public IFormFile Path { get; set; }

        public string AttachType { get; set; }

    }
}
