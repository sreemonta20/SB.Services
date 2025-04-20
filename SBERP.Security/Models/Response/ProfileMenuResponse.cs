using SBERP.Security.Models.Configuration;

namespace SBERP.Security.Models.Response
{
    /// <summary>
    /// It is used to keep and send the user profile and user menu information after successful authentication.
    /// </summary>
    public class ProfileMenuResponse
    {
        public User? user { get; set; }
        public string? userMenus { get; set; }
    }
}
