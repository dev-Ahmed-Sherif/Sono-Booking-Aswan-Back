using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Integration.File
{
    [ExcludeFromCodeCoverage]
    public class FileDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string FileSize { get; set; }
        public string Name { get; set; }
        public bool IsPublic { get; set; }
        public string DocumentType { get; set; }
        public string ContentType { get; set; }
    }
}

