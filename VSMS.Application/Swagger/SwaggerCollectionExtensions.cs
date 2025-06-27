using Microsoft.OpenApi.Models;

namespace VSMS.Application.Swagger;

public static class SwaggerCollectionExtensions
{
    public static WebApplicationBuilder AddSwaggerConfiguration(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "API - Virtual Stock Market Simulator", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new()
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT with Bearer into field. Example: \"Bearer {token}\"",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new()
            {
                {
                    new()
                    {
                        Reference = new()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
                
            c.OperationFilter<SwaggerFileOperationFilter>();
        });
        
        return builder;
    }
}