namespace SB.Security.Models.Base
{
    /// <summary>
    /// This is a token object used after successful authentication.
    /// </summary>
    public class AccessToken
    {
        public string? Token_Type { get; set; }
        public string? Access_Token { get; set; }
        public DateTime? Expires_In { get; set; }
    }
}
