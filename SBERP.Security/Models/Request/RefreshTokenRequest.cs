using System.ComponentModel.DataAnnotations;

namespace SBERP.Security.Models.Request
{
    public class RefreshTokenRequest
    {
        //public string? UserName { get; set; }
        /// <summary>The (usually expired) access token of the same session; proves the refresh token is used by its owner.</summary>
        [Required]
        public string? Access_Token { get; set; }
        [Required]
        public string? Refresh_Token { get; set; }
    }
}
