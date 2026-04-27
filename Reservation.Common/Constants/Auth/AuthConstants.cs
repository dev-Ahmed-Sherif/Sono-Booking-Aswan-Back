namespace SonoTracker.Common.Constants.Auth
{
    public static class AuthConstants
    {
        public const string AccessTokenKey = "AccessToken";
        public const string RefreshTokenKey = "RefreshToken";
        public const int AccessTokenLifeInHours = 7;
        public const int RefreshTokenLife = 7;
        public const string DefaultPassword = "12345";
        public const string Permissions = nameof(Permissions);
        public const string OrgId = nameof(OrgId);
        public const string FloatingUnitId = nameof(FloatingUnitId);
        public const string GovId = nameof(GovId);
    }
}
