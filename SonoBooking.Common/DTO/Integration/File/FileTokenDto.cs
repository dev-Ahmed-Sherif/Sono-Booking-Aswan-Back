using System;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Integration.File
{
    [ExcludeFromCodeCoverage]
    public class FileTokenDto
    {
        public Guid FileId { get; set; }
        public string Token { get; set; }
    }
}

