using SB.Security.Models.Base;
using SB.Security.Models.Configuration;
using System.Security.Claims;

namespace SB.Security.Service
{
    /// <summary>
    /// It define all the methods methods inclduing generation of jwt token, refresh token, and getting claim principals. Where <see cref="TokenService"/> implements this interface.
    /// </summary>
    public interface ITokenService
    {
        Token? GetToken(AppUserProfile user);
        Token? GenerateAccessToken(AppUserProfile user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
