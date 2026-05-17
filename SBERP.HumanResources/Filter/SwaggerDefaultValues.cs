using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace SBERP.HumanResources.Filter
{
    /// <summary>
    /// Swashbuckle 10.x + Asp.Versioning 10.x operation filter.
    /// Removes the auto-injected "api-version" query parameter from Swagger UI,
    /// sets deprecated flag on deprecated API versions, and fixes parameter
    /// descriptions that are missing in the default metadata.
    ///
    /// Register in Program.cs:
    ///   c.OperationFilter<SwaggerDefaultValues>();
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