using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SB.Security.Helper;

namespace SB.Security.Filter
{
    /// <summary>
    /// It validates model.
    /// </summary>
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        #region All methods
        /// <summary>
        /// It is not being used properly for validating the model. Once required fields are set in the model classes then it will automatically checks.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns>void</returns>
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (actionContext.ActionArguments.Any(kv => kv.Value == null))
            {
                actionContext.Result = new BadRequestObjectResult(ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY);
                return;
            }

            if (!actionContext.ModelState.IsValid)
            {
                var message = string.Join(" | ", actionContext.ModelState.Values
                                  .SelectMany(v => v.Errors)
                                  .Select(e => e.ErrorMessage));
                if (string.IsNullOrEmpty(message)) message = ConstantSupplier.REQUIRED_PARAMETER_NOT_EMPTY;

                actionContext.Result = new BadRequestObjectResult(message);
            }

        }
        #endregion
    }
}
