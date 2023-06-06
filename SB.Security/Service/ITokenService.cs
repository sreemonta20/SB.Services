using SB.Security.Models.Base;
using SB.Security.Models.Configuration;
using System.Security.Claims;

namespace SB.Security.Service
{
    public interface ITokenService
    {
        Token? GetToken(UserInfo user);
        Token? GenerateAccessToken(UserInfo user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
