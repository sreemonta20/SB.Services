using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace SBERP.Shared.Authorization
{
    /// <summary>
    /// The endpoint needs one of the listed rights on at least one of the listed screens. Put it on a
    /// controller for all its actions; one on an action replaces the controller's. List several
    /// screens for an endpoint that more than one screen calls (e.g. a list used as a dropdown).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class RequireScreenAttribute(params string[] screens) : Attribute
    {
        public string[] Screens { get; } = screens;
        public ScreenAction Action { get; set; } = ScreenAction.FromHttpMethod;
    }

    /// <summary>
    /// Any signed-in user may call this endpoint: reference data for dropdowns, or a "my ..." endpoint
    /// that works only on the caller's own data. The decision must be stated: see <see cref="ScreenPermissionFilter"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AuthenticatedOnlyAttribute : Attribute;

    /// <summary>
    /// Enforces menu rights on the server, for every controller action. It denies by default: an
    /// action with neither <see cref="RequireScreenAttribute"/>, <see cref="AuthenticatedOnlyAttribute"/>
    /// nor [AllowAnonymous] is refused, so a new endpoint cannot be left open by forgetting a check.
    /// </summary>
    public sealed class ScreenPermissionFilter(ILogger<ScreenPermissionFilter> logger) : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IList<object> metadata = context.ActionDescriptor.EndpointMetadata;
            if (metadata.OfType<IAllowAnonymous>().Any())
                return;

            // endpoint metadata lists the controller's attributes first, the action's last: the action's wins
            object? rule = metadata.LastOrDefault(m => m is RequireScreenAttribute or AuthenticatedOnlyAttribute);
            if (rule is AuthenticatedOnlyAttribute)
                return;

            HttpContext http = context.HttpContext;
            string action = context.ActionDescriptor.DisplayName ?? http.Request.Path;
            if (rule is not RequireScreenAttribute required)
            {
                logger.LogError("Endpoint {Action} declares no permission rule and was refused.", action);
                context.Result = Forbidden();
                return;
            }

            // a token issued before permissions were added to it: have the client renew it, not fail
            if (http.User.FindFirst(SberpClaims.Permissions) == null)
            {
                http.Response.Headers.Append("Token-Expired", "true");
                context.Result = new UnauthorizedResult();
                return;
            }

            ScreenAction needed = required.Action != ScreenAction.FromHttpMethod
                ? required.Action
                : FromHttpMethod(http.Request.Method);
            if (!required.Screens.Any(screen => http.User.HasScreen(screen, needed)))
            {
                logger.LogWarning("Permission denied: user {UserId} on {Action} (needs {Needed} on {Screens}).",
                    http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, action, needed, required.Screens);
                context.Result = Forbidden();
            }
        }

        private static ScreenAction FromHttpMethod(string method) =>
            HttpMethods.IsGet(method) || HttpMethods.IsHead(method) ? ScreenAction.View
            : HttpMethods.IsDelete(method) ? ScreenAction.Delete
            : ScreenAction.Create | ScreenAction.Update;

        /// <summary>The body goes through the service's own JSON settings (Security PascalCase, HR camelCase).</summary>
        private static ObjectResult Forbidden() => new(new
        {
            Success = false,
            Message = "You do not have permission to do this.",
            ResponseCode = StatusCodes.Status403Forbidden
        })
        { StatusCode = StatusCodes.Status403Forbidden };
    }

    public static class ScreenPermissionExtensions
    {
        /// <summary>
        /// Adds the permission filter to every controller action. Authorization:EnforceScreenPermissions
        /// (default true) can switch it off as an emergency measure; it should stay on.
        /// </summary>
        public static IServiceCollection AddScreenPermissions(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue("Authorization:EnforceScreenPermissions", true))
                services.Configure<MvcOptions>(options => options.Filters.Add<ScreenPermissionFilter>());
            services.Configure<MvcOptions>(options => options.Filters.Add<AuditActorFilter>());
            return services;
        }
    }
}
