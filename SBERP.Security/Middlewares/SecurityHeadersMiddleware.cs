namespace SBERP.Security.Middlewares
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Configure security headers from app settings
            var securityHeaders = _configuration.GetSection("SecurityHeaders");

            // Content-Security-Policy
            var csp = securityHeaders["ContentSecurityPolicy"];
            if (!string.IsNullOrEmpty(csp))
                context.Response.Headers.TryAdd("Content-Security-Policy", csp);

            // X-Content-Type-Options
            context.Response.Headers.TryAdd("X-Content-Type-Options",
                string.IsNullOrEmpty(securityHeaders["XContentTypeOptions"]) ? "nosniff" : securityHeaders["XContentTypeOptions"]);

            // X-Frame-Options
            context.Response.Headers.TryAdd("X-Frame-Options",
                string.IsNullOrEmpty(securityHeaders["XFrameOptions"]) ? "DENY" : securityHeaders["XFrameOptions"]);

            // X-XSS-Protection
            context.Response.Headers.TryAdd("X-XSS-Protection", "1; mode=block");

            // Referrer-Policy
            context.Response.Headers.TryAdd("Referrer-Policy",
                string.IsNullOrEmpty(securityHeaders["ReferrerPolicy"]) ? "no-referrer" : securityHeaders["ReferrerPolicy"]);

            // Permissions-Policy
            context.Response.Headers.TryAdd("Permissions-Policy",
                string.IsNullOrEmpty(securityHeaders["PermissionsPolicy"]) ?
                    "camera=(), microphone=(), geolocation=()" :
                    securityHeaders["PermissionsPolicy"]);

            // Strict-Transport-Security
            if (context.Request.IsHttps)
            {
                context.Response.Headers.TryAdd("Strict-Transport-Security",
                    string.IsNullOrEmpty(securityHeaders["StrictTransportSecurity"]) ?
                        "max-age=31536000; includeSubDomains" :
                        securityHeaders["StrictTransportSecurity"]);
            }

            // Remove headers that might leak info
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");
            context.Response.Headers.Remove("X-AspNet-Version");
            context.Response.Headers.Remove("X-AspNetMvc-Version");

            await _next(context);
        }
    }
}
