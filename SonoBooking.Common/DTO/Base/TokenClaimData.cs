using System.Diagnostics.CodeAnalysis;

namespace SonoTracker.Common.DTO.Base
{
    [ExcludeFromCodeCoverage]
    public class TokenClaimDto
    {
        public string UserId { get; set; }
        public string UnitId { get; set; }
        public string TeamId { get; set; }
        public string Email { get; set; }
        public string EmployeeEn { get; set; }
        public string EmployeeAr { get; set; }
        public string EmployeeId { get; set; }
        public string IpAddress { get; set; }
        public string NationalId { get; set; }

    }
    [ExcludeFromCodeCoverage]
    public class ClaimDto
    {
        public string AppName { get; set; }
        public long AppId { get; set; }
        public string RoleName { get; set; }
        public string RoleNameAr { get; set; }
        public string RoleCode { get; set; }
        public string AppCode { get; set; }
        public long RoleId { get; set; }
    }
}