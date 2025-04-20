using SBERP.Security.Models.Base;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Models.Request;
using System.Security.Claims;

namespace SBERP.Security.Service
{
    /// <summary>
    /// It define all the methods methods inclduing generation of jwt token, refresh token, and getting claim principals. Where <see cref="TokenService"/> implements this interface.
    /// </summary>
    public interface ITokenService
    {
        //Token? GeneretateJWT(User? user);
        
        //ClaimsPrincipal GetPrincipalFromExpiredToken(string? token);
        string GenerateJwtToken(ClaimRequest oClaimRequest);
        string GenerateRefreshToken();
    }
}
