using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SBERP.Shared.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SBERP.Shared.Middleware
{
    /// <summary>
    /// Register in Program.cs BETWEEN UseAuthentication() and UseAuthorization():
    ///
    ///   app.UseAuthentication();   <-- validates JWT signature + expiry
    ///   app.UseTokenBlacklist();   <-- THIS middleware
    ///   app.UseAuthorization();    <-- enforces [Authorize] attributes
    ///
    /// </summary>
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenBlacklistMiddleware> _logger;

        public TokenBlacklistMiddleware(
            RequestDelegate next,
            ILogger<TokenBlacklistMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ITokenBlacklistService blacklistService)
        {
            // Only check authenticated requests — anonymous endpoints pass through
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var rawToken = context.Request.Headers["Authorization"]
                    .FirstOrDefault()
                    ?.Replace("Bearer ", string.Empty,
                              StringComparison.OrdinalIgnoreCase);

                if (!string.IsNullOrEmpty(rawToken))
                {
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        if (handler.CanReadToken(rawToken))
                        {
                            var jwt = handler.ReadJwtToken(rawToken);
                            var jti = jwt.Id; // JwtRegisteredClaimNames.Jti

                            if (!string.IsNullOrEmpty(jti) &&
                                await blacklistService.IsBlacklistedAsync(jti))
                            {
                                _logger.LogWarning(
                                    "Blacklisted token (JTI:{Jti}) blocked on {Path}",
                                    jti, context.Request.Path);

                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";

                                await context.Response.WriteAsync(
                                    JsonSerializer.Serialize(new
                                    {
                                        Success = false,
                                        Message = "Token has been revoked. Please log in again.",
                                        ResponseCode = 401
                                    }));
                                return; // short-circuit — do NOT call next()
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Parsing failure — log and allow through (fail open)
                        _logger.LogWarning(ex,
                            "Could not parse JWT in TokenBlacklistMiddleware.");
                    }
                }
            }

            // Token is valid and not blacklisted — pass to next middleware
            await _next(context);
        }
    }
}
