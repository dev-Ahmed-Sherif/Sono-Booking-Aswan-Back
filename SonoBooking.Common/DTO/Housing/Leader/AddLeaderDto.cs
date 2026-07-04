using Microsoft.AspNetCore.Http;
using SonoBooking.Common.Core;
using System.Diagnostics.CodeAnalysis;

namespace SonoBooking.Common.DTO.Housing.Leader
{
    [ExcludeFromCodeCoverage]
    public class AddLeaderDto : IEntityDto<string>
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public bool IsActive { get; set; }
        public IFormFile File { get; set; }
    }
}
