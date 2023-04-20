namespace SB.Security.Models.Configuration
{
    /// <summary>
    /// It is to track the JWT Token settings in  <see cref="AppSettings"/>.
    /// </summary>
    public class JWT
    {
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public string? Subject { get; set; }
    }
}