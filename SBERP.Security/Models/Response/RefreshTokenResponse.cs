namespace SBERP.Security.Models.Response
{
    public class RefreshTokenResponse
    {
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
        public DateTime? expires_in { get; set; }
    }
}
