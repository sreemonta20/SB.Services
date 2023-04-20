namespace SB.Security.Middlewares
{
    /// <summary>
    /// Extension method used to add the middleware to the HTTP request pipeline.
    /// </summary>
    public static class SecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseEncDecMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EncDecMiddleware>();
        }
    }
}
