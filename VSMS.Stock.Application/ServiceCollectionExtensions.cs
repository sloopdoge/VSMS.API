using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace VSMS.Stock.Application;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddStockService(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        
        var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");

        return builder;
    }
}