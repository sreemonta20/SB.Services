using Newtonsoft.Json.Linq;
using SBERP.Security.Models.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SBERP.Security.Models.Response
{
    /// <summary>
    /// It is used to track and display the token related information after successful authentication.
    /// </summary>
    public class AuthResponse(User? user, string? userMenus, string? accessToken, string? refreshToken, DateTime expiresIn,
    string? tokenType, string? error = null, string? errorDescription = null)
    {
        private User? _user { get; set; } = user;
        private string? _userMenus { get; set; } = userMenus;
        private string? _access_token { get; set; } = accessToken;
        private string? _refresh_token { get; set; } = refreshToken;
        private DateTime _expires_in { get; set; } = expiresIn;
        private string? _token_type { get; set; } = tokenType;
        private string? _error { get; set; } = error;
        private string? _error_description { get; set; } = errorDescription;

        public User? user
        {
            get => _user;
            set => _user = value;
        }

        public string? userMenus
        {
            get => _userMenus;
            set => _userMenus = value;
        }

        public string? access_token
        {
            get => _access_token;
            set => _access_token = value;
        }

        public string? refresh_token
        {
            get => _refresh_token;
            set => _refresh_token = value;
        }

        public DateTime expires_in
        {
            get => _expires_in;
            set => _expires_in = value;
        }

        public string? token_type
        {
            get => _token_type;
            set => _token_type = value;
        }

        public string? error
        {
            get => _error;
            set => _error = value;
        }

        public string? error_description
        {
            get => _error_description;
            set => _error_description = value;
        }
    }
}
