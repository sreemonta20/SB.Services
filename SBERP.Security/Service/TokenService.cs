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
    //public class TokenService : ITokenService
    //{
    //    public readonly IConfiguration _configuration;
    //    public TokenService(IConfiguration config)
    //    {
    //        _configuration = config;
    //    }


    //    public Token? GetToken(User? user)
    //    {

    //        JwtSecurityTokenHandler tokenHandler = new();
    //        byte[] key = Encoding.ASCII.GetBytes(_configuration["AppSettings:JWT:Key"]);

    //        DateTime expiryTime = DateTime.Now.AddSeconds(Convert.ToDouble(this._configuration["AppSettings:AccessTokenExpireTime"]));
    //        SecurityTokenDescriptor tokenDescriptor = new()
    //        {
    //            Subject = new ClaimsIdentity(new Claim[]
    //            {
    //                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["AppSettings:JWT:Subject"]),
    //                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
    //                    new Claim("UserId", user.Id.ToString()),
    //                    new Claim("FullName", user.FullName),
    //                    new Claim(ClaimTypes.Name, user.UserName),
    //                    new Claim("Email", user.Email)
    //            }),
    //            Expires = expiryTime,
    //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //        };

    //        var token = tokenHandler.CreateToken(tokenDescriptor);
    //        var tokenString = tokenHandler.WriteToken(token);

    //        if (tokenString != null)
    //        {
    //            return new Token()
    //            {
    //                access_token = tokenString,
    //                expires_in = expiryTime,
    //                token_type = ConstantSupplier.AUTHORIZATION_TOKEN_TYPE,
    //                error = string.Empty,
    //                error_description = string.Empty,
    //                user = new User() { Id = Convert.ToString(user.Id), FullName = user.UserName, UserName = user.UserName, Email = user.Email, UserRole = user.UserRole.ToString(), CreatedDate = user.CreatedDate }

    //            };
    //        }
    //        return null;


    //    }

    //    public Token? GenerateAccessToken(User user)
    //    {
    //        List<Claim> claims = new()
    //        {
    //            new Claim(JwtRegisteredClaimNames.Sub, this._configuration["AppSettings:JWT:Subject"]),
    //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
    //            new Claim("UserId", user.Id.ToString()),
    //            new Claim("FullName", user.FullName),
    //            new Claim("UserName", user.UserName),
    //            new Claim("Email", user.Email)
    //        };
    //        SymmetricSecurityKey secretKey = new(Encoding.UTF8.GetBytes(_configuration["AppSettings:JWT:Key"]));
    //        SigningCredentials signinCredentials = new(secretKey, SecurityAlgorithms.HmacSha256);
    //        DateTime expiryTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["AppSettings:Expires"]));

    //        JwtSecurityToken tokenOptions = new(
    //            issuer: _configuration["AppSettings:JWT:Issuer"],
    //            audience: _configuration["AppSettings:JWT:Audience"],
    //            claims: claims,
    //            expires: expiryTime,
    //            signingCredentials: signinCredentials
    //        );

    //        string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    //        if (tokenString != null)
    //        {
    //            return new Token()
    //            {
    //                access_token = tokenString,
    //                refresh_token = GenerateRefreshToken(),
    //                expires_in = expiryTime,
    //                token_type = ConstantSupplier.AUTHORIZATION_TOKEN_TYPE,
    //                error = string.Empty,
    //                error_description = string.Empty,
    //                user = new User() { Id = Convert.ToString(user.Id), FullName = user.UserName, UserName = user.UserName, Email = user.Email, UserRole = user.UserRole, CreatedDate = user.CreatedDate }

    //            };
    //        }
    //        return null;
    //    }

    //    public string GenerateRefreshToken()
    //    {
    //        byte[] randomNumber = new byte[32];
    //        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
    //        rng.GetBytes(randomNumber);
    //        return Convert.ToBase64String(randomNumber);
    //    }

    //    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    //    {
    //        TokenValidationParameters tokenValidationParameters = new()
    //        {
    //            ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
    //            ValidateIssuer = false,
    //            ValidateIssuerSigningKey = true,
    //            ClockSkew = TimeSpan.Zero,
    //            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:JWT:Key"])),
    //            ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
    //        };

    //        JwtSecurityTokenHandler tokenHandler = new();
    //        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
    //        JwtSecurityToken? jwtSecurityToken = securityToken as JwtSecurityToken;
    //        return jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)
    //            ? throw new SecurityTokenException("Invalid token")
    //            : principal;
    //    }
    //}
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
