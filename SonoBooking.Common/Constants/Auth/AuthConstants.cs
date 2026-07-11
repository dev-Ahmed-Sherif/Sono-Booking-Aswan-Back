namespace SonoBooking.Common.Constants.Auth
{
    public static class AuthConstants
    {
        public const string AccessTokenKey = "AccessToken";
        public const string RefreshTokenKey = "RefreshToken";
        public const int AccessTokenLife = 7;
        public const int RefreshTokenLife = 30;
        public const string DefaultPassword = "12345";
        public const string Permissions = nameof(Permissions);
        public const string OrgId = nameof(OrgId);
        public const string LeaderId = nameof(LeaderId);
        public const string GovId = nameof(GovId);
        public const string EmployeeId = nameof(EmployeeId);
    }
}

