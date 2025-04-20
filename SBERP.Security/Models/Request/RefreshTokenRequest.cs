namespace SBERP.Security.Models.Request
{
    public class RefreshTokenRequest
    {
        //public string? UserName { get; set; }
        public string? Access_Token { get; set; }
        public string? Refresh_Token { get; set; }
    }
}
