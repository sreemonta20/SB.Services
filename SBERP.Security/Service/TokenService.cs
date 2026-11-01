using Microsoft.IdentityModel.Tokens;
using SBERP.Security.Helper;
using SBERP.Security.Models.Base;
using SBERP.Security.Models.Configuration;
using SBERP.Security.Models.Request;
using SBERP.Shared.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
            _expiresInTime = Convert.ToInt32(_configuration["AppSettings:AccessTokenExpirationMinutes"]);
        }

        
        public string GenerateJwtToken(ClaimRequest oClaimRequest)
        {
            var oClaim = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, oClaimRequest.UserId!),
                new(ClaimTypes.Name, oClaimRequest.UserName!),
                new(ClaimTypes.Email, oClaimRequest.Email ?? string.Empty),
                new(ClaimTypes.Role, oClaimRequest.Role!),
                // unique token id: required for the Redis blacklist on logout
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                // an empty value still counts: it means "no rights", which is not the same as an old token without the claim
                new(SberpClaims.Permissions, oClaimRequest.Permissions ?? string.Empty)
            };
            if (!string.IsNullOrEmpty(oClaimRequest.ProfileId))
                oClaim.Add(new Claim(SberpClaims.ProfileId, oClaimRequest.ProfileId));
            if (!string.IsNullOrEmpty(oClaimRequest.EmployeeId))
                oClaim.Add(new Claim(SberpClaims.EmployeeId, oClaimRequest.EmployeeId));

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

        /// <summary>
        /// 512 random bits, base64url (safe in JSON, headers and URLs). Only its hash is stored (<see cref="HashToken"/>).
        /// </summary>
        public string GenerateRefreshToken()
        {
            return Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));
        }

        /// <summary>
        /// SHA-256 of a refresh token, upper-case hex. The token is already 512 random bits, so a fast
        /// hash is enough (no salt / BCrypt needed) and it can be looked up by an index.
        /// </summary>
        public string HashToken(string token)
        {
            return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        }

        /// <summary>
        /// Validates an access token issued by this service (signature, issuer, audience, algorithm) while
        /// ignoring its expiry, so the refresh endpoint can tell whose session an expired token belongs to.
        /// Returns <c>null</c> when the token is missing, forged or malformed.
        /// </summary>
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
                return null;

            TokenValidationParameters parameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = false,
                ValidIssuer = _jwtIssuer,
                ValidAudience = _jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey!)),
                ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
            };

            try
            {
                return new JwtSecurityTokenHandler().ValidateToken(accessToken, parameters, out _);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
