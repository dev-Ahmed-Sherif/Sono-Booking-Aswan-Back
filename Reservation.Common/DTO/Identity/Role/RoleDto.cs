using System.Diagnostics.CodeAnalysis;
using SonoTracker.Common.Core;

namespace SonoTracker.Common.DTO.Identity.Role
{
    [ExcludeFromCodeCoverage]
    public class RoleDto
    {
        public string Id { get; set; }
        public string NameEn { get; set; }
        public string NameAr { get; set; }
        //public string AppNameEn { get; set; }
        //public string AppNameAr { get; set; }
        //public string AppCode { get; set; }
        //public string Code { get; set; }
    }
}
