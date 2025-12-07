using Microsoft.IdentityModel.Tokens;
using SBERP.Security.Helper;
using SBERP.Security.Models.Base;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Models.Request;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace SBERP.Security.Service
{
    /// <summary>
    /// It implements <see cref="ITokenService"/> all the methods inclduing generation of jwt token, refresh token, and getting claim principals are defined.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly string? _jwtKey;
        private readonly string? _jwtIssuer;
        private readonly string? _jwtAudience;
        private readonly string? _jwtSubject;
        private readonly int _expiresInTime;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _jwtKey = _configuration["AppSettings:JWT:Key"];
            _jwtIssuer = _configuration["AppSettings:JWT:Issuer"];
            _jwtAudience = _configuration["AppSettings:JWT:Audience"];
            _jwtSubject = _configuration["AppSettings:JWT:Subject"];
            //_expiresInTime = Convert.ToInt32(_configuration["AppSettings:Expires"]);
            _expiresInTime = Convert.ToInt32(_configuration["AppSettings:AccessTokenExpirationMinutes"]);
        }

        
        public string GenerateJwtToken(ClaimRequest oClaimRequest)
        {
            var oClaim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, oClaimRequest.UserId!.ToString()),
                new Claim(ClaimTypes.Name, oClaimRequest.UserName!),
                new Claim(ClaimTypes.Email, oClaimRequest.Email!),
                new Claim(ClaimTypes.Role, oClaimRequest.Role!)
            };

            SymmetricSecurityKey oSymmetricSecurityKey = new(Encoding.UTF8.GetBytes(_jwtKey!));
            SigningCredentials oSigningCredentials = new(oSymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            DateTime oExpiryDateTime = DateTime.UtcNow.AddMinutes(_expiresInTime);

            JwtSecurityToken oJwtSecurityToken = new(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: oClaim,
                expires: oExpiryDateTime,
                signingCredentials: oSigningCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(oJwtSecurityToken);
        }

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
