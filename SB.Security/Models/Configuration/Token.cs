namespace SB.Security.Models.Configuration
{
    /// <summary>
    /// It is used to track and display the token related information after successful authentication.
    /// </summary>
    public class Token
    {
        public string? access_token { get; set; }
        public int expires_in { get; set; }
        public string? token_type { get; set; }
        public string?  error { get; set; }
        public string? error_description { get; set; }
        public User? user { get; set; }
    }
}
