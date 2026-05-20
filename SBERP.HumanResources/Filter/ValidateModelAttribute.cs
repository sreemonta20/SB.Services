using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SBERP.HumanResources.Helper;

namespace SBERP.HumanResources.Filter
{
    /// <summary>
    /// Catches null arguments and model state errors before they reach the
    /// controller body. Registered via ServiceFilter on each action.
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (ctx.ActionArguments.Any(kv => kv.Value == null))
            {
                ctx.Result = new BadRequestObjectResult(
                    ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);
                return;
            }

            if (!ctx.ModelState.IsValid)
            {
                var message = string.Join(" | ", ctx.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                if (string.IsNullOrEmpty(message))
                    message = ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY;

                ctx.Result = new BadRequestObjectResult(message);
            }
        }
    }
}
