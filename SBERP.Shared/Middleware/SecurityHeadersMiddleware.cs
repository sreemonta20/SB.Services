using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SBERP.Shared.Middleware
{
    /// <summary>
    /// Security headers on every response, overridable through the "SecurityHeaders" configuration
    /// section. The defaults suit a JSON API that is never framed and never rendered as a page.
    /// </summary>
    public sealed class SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        private readonly IConfigurationSection _settings = configuration.GetSection("SecurityHeaders");

        public Task InvokeAsync(HttpContext context)
        {
            IHeaderDictionary headers = context.Response.Headers;
            string? csp = _settings["ContentSecurityPolicy"];
            if (!string.IsNullOrEmpty(csp))
                headers.TryAdd("Content-Security-Policy", csp);
            headers.TryAdd("X-Content-Type-Options", Setting("XContentTypeOptions", "nosniff"));
            headers.TryAdd("X-Frame-Options", Setting("XFrameOptions", "DENY"));
            headers.TryAdd("Referrer-Policy", Setting("ReferrerPolicy", "no-referrer"));
            headers.TryAdd("Permissions-Policy", Setting("PermissionsPolicy", "camera=(), microphone=(), geolocation=()"));
            if (context.Request.IsHttps)
                headers.TryAdd("Strict-Transport-Security", Setting("StrictTransportSecurity", "max-age=31536000; includeSubDomains"));
            return next(context);
        }

        private string Setting(string name, string fallback)
        {
            string? value = _settings[name];
            return string.IsNullOrEmpty(value) ? fallback : value;
        }
    }
}
