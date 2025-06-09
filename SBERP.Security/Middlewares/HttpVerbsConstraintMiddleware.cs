using Newtonsoft.Json;
using SBERP.Security.Models.Response;
using System.Net;

namespace SBERP.Security.Middlewares
{
    public class HttpVerbsConstraintMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HashSet<string> _allowedMethods;

        public HttpVerbsConstraintMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;

            var verbs = configuration.GetSection("AllowedHttpVerbs").Get<List<string>>() ?? new List<string>();
            _allowedMethods = new HashSet<string>(verbs, StringComparer.OrdinalIgnoreCase);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!_allowedMethods.Contains(context.Request.Method))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;

                var response = new DataResponse
                {
                    Success = false,
                    Message = "The requested HTTP method is not allowed.",
                    MessageType = SBERP.Security.Enum.EnumResponseType.Error,
                    ResponseCode = (int)HttpStatusCode.MethodNotAllowed,
                    Result = null
                };

                await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                return;
            }

            await _next(context);
        }
    }
}
