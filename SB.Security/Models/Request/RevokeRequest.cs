namespace SB.Security.Models.Request
{
    public class RevokeRequest
    {
        public string? Access_Token { get; set; }
        public string? Refresh_Token { get; set; }
    }
}
