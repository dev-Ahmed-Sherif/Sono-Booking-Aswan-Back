namespace SonoTracker.Common.DTO.Identity.User
{
    public class LoginResponseDto
    {
        public bool IsLogedIn { get; set; } = false;
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
