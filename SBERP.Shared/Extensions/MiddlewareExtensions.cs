using Microsoft.AspNetCore.Builder;
using SBERP.Shared.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace SBERP.Shared.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Register in every microservice's Program.cs pipeline:
        ///
        ///   app.UseAuthentication();
        ///   app.UseTokenBlacklist();   <-- call this
        ///   app.UseAuthorization();
        ///
        /// </summary>
        public static IApplicationBuilder UseTokenBlacklist(
            this IApplicationBuilder app)
            => app.UseMiddleware<TokenBlacklistMiddleware>();
    }
}
