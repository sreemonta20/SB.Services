// Microsoft.OpenApi 2.x+ (used by Swashbuckle 10.x) breaking changes vs 1.x:
//
// REMOVED: using Microsoft.OpenApi.Any       → OpenApiString gone, use JsonValue
// REMOVED: using Microsoft.OpenApi.Models    → namespace gone, use Microsoft.OpenApi
// CHANGED: operation.Parameters              → IList<IOpenApiParameter> (read-only interface)
//          Cast to OpenApiParameter to access setters
// CHANGED: parameter.Schema.Default          → was IOpenApiAny, now JsonNode?
//          Use JsonValue.Create() instead of new OpenApiString()
// CHANGED: parameter.Required                → read-only on IOpenApiParameter interface
//          Cast to OpenApiParameter to access setter

using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace SBERP.Security.Filter
{
    /// <summary>
    /// Applies API default values to Swagger operation parameters.
    /// Updated for Swashbuckle 10.x / Microsoft.OpenApi 2.x+ / .NET 10.
    /// </summary>
    public class SwaggerDefaultValues : IOperationFilter
    {
        #region All methods
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            if (operation.Parameters == null)
            {
                return;
            }

            foreach (var parameter in operation.Parameters)
            {
                // Safe lookup — Asp.Versioning injects synthetic parameters (e.g. api-version)
                // that don't exist in ParameterDescriptions. FirstOrDefault + null check
                // handles them safely. The original .First() would throw here.
                var description = apiDescription.ParameterDescriptions
                    .FirstOrDefault(p => p.Name == parameter.Name);

                if (description == null) continue;

                // Cast required: IOpenApiParameter interface properties are read-only.
                // OpenApiParameter concrete class has the setters.
                if (parameter is not OpenApiParameter concreteParameter) continue;

                if (concreteParameter.Description == null)
                    concreteParameter.Description = description.ModelMetadata?.Description;

                // Cast Schema to OpenApiSchema (concrete) — IOpenApiSchema.Default is read-only.
                // JsonValue.Create replaces new OpenApiString() which was removed in OpenApi 2.x.
                if (concreteParameter.Schema is OpenApiSchema concreteSchema
                    && concreteSchema.Default == null
                    && description.DefaultValue != null)
                {
                    concreteSchema.Default = JsonValue.Create(description.DefaultValue.ToString());
                }

                concreteParameter.Required |= description.IsRequired;
            }
        }
        #endregion
    }
}