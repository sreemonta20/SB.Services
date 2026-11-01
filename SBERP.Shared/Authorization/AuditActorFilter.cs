using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;
using System.Reflection;

namespace SBERP.Shared.Authorization
{
    /// <summary>
    /// The "who did this" field of a request (CreateUpdateBy) is filled in by the browser, so a caller
    /// could record the change under someone else's name. This overwrites it with the profile id of
    /// the signed-in user, so the audit columns and log tables can be trusted.
    /// </summary>
    public sealed class AuditActorFilter : IActionFilter
    {
        private static readonly string[] ActorProperties = ["CreateUpdateBy", "CreateUpdatedBy"];
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> Cache = new();

        public void OnActionExecuting(ActionExecutingContext context)
        {
            string? actor = context.HttpContext.User.FindFirst(SberpClaims.ProfileId)?.Value;
            if (string.IsNullOrEmpty(actor))
                return; // anonymous endpoints: nothing to take the actor from

            foreach (object? argument in context.ActionArguments.Values)
            {
                if (argument == null)
                    continue;
                foreach (PropertyInfo property in Cache.GetOrAdd(argument.GetType(), FindActorProperties))
                    property.SetValue(argument, actor);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        private static PropertyInfo[] FindActorProperties(Type type) => type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.PropertyType == typeof(string) && ActorProperties.Contains(p.Name))
            .ToArray();
    }
}
