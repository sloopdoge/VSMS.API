using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VSMS.Application.Swagger;

public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var parameter in operation.Parameters.Where(p => p.In == ParameterLocation.Query))
        {
            var fileParameter = context.ApiDescription.ParameterDescriptions
                .FirstOrDefault(p => p.ModelMetadata?.ModelType == typeof(IFormFile));

            if (fileParameter != null)
            {
                operation.RequestBody = new()
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new()
                        {
                            Schema = new()
                            {
                                Type = "object",
                                Properties =
                                {
                                    [fileParameter.Name] = new()
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                },
                                Required = new HashSet<string> { fileParameter.Name }
                            }
                        }
                    }
                };
            }
        }
    }
}