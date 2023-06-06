namespace SB.Security.Models.Request
{
    public class RefreshTokenRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Access_Token { get; set; }
        public string? Refresh_Token { get; set; }
    }
}
