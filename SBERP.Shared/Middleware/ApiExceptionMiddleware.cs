using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SBERP.Shared.Middleware
{
    /// <summary>
    /// Last line of defence: an exception nothing else handled is logged in full and the client gets a
    /// generic message (a database error can name tables, servers and users). Exception details are
    /// added only in the Development environment.
    /// <para>
    /// Everything is a 500 on purpose. Mapping UnauthorizedAccessException to 401 signs the user out
    /// when the real problem is a file permission on the server, because the .NET file APIs throw it.
    /// </para>
    /// </summary>
    public sealed class ApiExceptionMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionMiddleware> logger,
        IHostEnvironment environment,
        JsonNamingPolicy? namingPolicy = null)
    {
        public const string GenericMessage = "An unexpected error occurred. Please try again or contact the administrator.";

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex) when (ex is not OperationCanceledException || !context.RequestAborted.IsCancellationRequested)
            {
                logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
                if (context.Response.HasStarted)
                    throw;

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    Success = false,
                    Message = GenericMessage,
                    ResponseCode = StatusCodes.Status500InternalServerError,
                    Result = environment.IsDevelopment() ? new { Exception = ex.GetType().FullName, ex.Message, ex.StackTrace } : null
                }, new JsonSerializerOptions { PropertyNamingPolicy = namingPolicy }));
            }
        }
    }
}
