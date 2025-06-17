using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VSMS.Company.Infrastructure.Interfaces;
using VSMS.Company.Infrastructure.Services;
using VSMS.Company.Repository;

namespace VSMS.Company.Application;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddCompanyService(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        
        var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<CompaniesDbContext>(options => 
            options.UseSqlServer(defaultConnectionString));

        services.AddScoped<ICompaniesService, CompaniesService>();

        return builder;
    }
}