using Microsoft.AspNetCore.Builder;
using SBERP.Shared.Middleware;
using System.Text.Json;

namespace SBERP.Shared.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Register in every microservice's Program.cs pipeline:
        ///
        ///   app.UseAuthentication();
        ///   app.UseTokenBlacklist();   &lt;-- call this
        ///   app.UseAuthorization();
        ///
        /// </summary>
        public static IApplicationBuilder UseTokenBlacklist(this IApplicationBuilder app)
            => app.UseMiddleware<TokenBlacklistMiddleware>();

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
            => app.UseMiddleware<SecurityHeadersMiddleware>();

        /// <summary>First in the pipeline. <paramref name="namingPolicy"/> matches the service's JSON (null = PascalCase).</summary>
        public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder app, JsonNamingPolicy? namingPolicy = null)
            => namingPolicy == null
                ? app.UseMiddleware<ApiExceptionMiddleware>()
                : app.UseMiddleware<ApiExceptionMiddleware>(namingPolicy);
    }
}
