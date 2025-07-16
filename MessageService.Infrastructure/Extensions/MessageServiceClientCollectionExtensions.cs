using MessageService.Infrastructure.Interfaces;
using MessageService.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace MessageService.Infrastructure.Extensions;

public static class MessageServiceCollectionExtensions
{
    public static WebApplicationBuilder AddMessageServiceIntegration(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services.AddScoped<IMessageServiceClient, MessageServiceClient>();
        
        return builder;
    }
}