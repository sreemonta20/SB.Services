using Newtonsoft.Json;
using SBERP.Security.Helper;
using SBERP.Security.Models.Response;
using SBERP.Security.Service;
using System.Net;

namespace SBERP.Security.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ISecurityLogService _securityLogService;
        public GlobalExceptionMiddleware(RequestDelegate next, ISecurityLogService securityLogService)
        {
            _next = next;
            _securityLogService = securityLogService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (UnauthorizedAccessException ex)
            {
                _securityLogService.LogError(String.Format("Unauthorized Access Exception: {0}", ex.Message));
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.Unauthorized, ConstantSupplier.GLOBAL_ERR_AUTH_FAILED_NO_PERMISSION_MSG);
            }
            catch (ApplicationException ex)
            {
                _securityLogService.LogError(String.Format("Application Exception: {0}", ex.Message));
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest, ConstantSupplier.GLOBAL_ERR_INVALID_INPUT_STATE_MSG);
            }
            catch (InvalidOperationException ex)
            {
                _securityLogService.LogError(String.Format("Invalid Operation Exception: {0}", ex.Message));
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest, ConstantSupplier.GLOBAL_ERR_INVALID_OPERATION_MSG);
            }
            catch (KeyNotFoundException ex)
            {
                _securityLogService.LogError(String.Format("Key Not Found Exception: {0}", ex.Message));
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.NotFound, ConstantSupplier.GLOBAL_ERR_RESOURCE_NOT_FOUND_MSG);
            }
            catch (Exception ex)
            {
                _securityLogService.LogError(String.Format("Unhandled Exception: {0}", ex.Message));
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, ConstantSupplier.GLOBAL_ERR_UNEXPECTED_MSG);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode, string defaultMessage)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new DataResponse
            {
                Success = false,
                Message = defaultMessage,
                MessageType = Enum.EnumResponseType.Error,
                ResponseCode = (int)statusCode,
                Result = null
            };

            // In development, we might want to include more error details
            #if DEBUG
            errorResponse.Result = new
            {
                ExceptionType = exception.GetType().Name,
                ExceptionMessage = exception.Message
            };
            #endif

            var jsonResponse = JsonConvert.SerializeObject(errorResponse);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
