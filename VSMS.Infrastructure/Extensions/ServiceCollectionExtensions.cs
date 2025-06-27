using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VSMS.Domain.Constants;
using VSMS.Domain.Entities;
using VSMS.Infrastructure.Identity;
using VSMS.Infrastructure.Interfaces;
using VSMS.Infrastructure.Services;
using VSMS.Repository;

namespace VSMS.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddIdentityConfiguration(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        
        var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationRepository>(options => 
                options.UseSqlServer(defaultConnectionString));
            
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationRepository>()
            .AddDefaultTokenProviders();
        
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings.GetValue<string>("SecretKey");
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                    ValidAudience = jwtSettings.GetValue<string>("Audience"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyNames.AdminOnly, 
                policy => policy.RequireRole(RoleNames.Admin));
            options.AddPolicy(PolicyNames.AdminOrCompanyAdmin, 
                policy => policy.RequireRole(RoleNames.Admin, RoleNames.CompanyAdmin));
            options.AddPolicy(PolicyNames.AdminOrCompanyAdminOrManager, 
                policy => policy.RequireRole(RoleNames.Admin, RoleNames.CompanyAdmin, RoleNames.CompanyManager));
        });

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthorizationHandler, CompanyOwnershipHandler>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddStocksConfiguration(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        
        var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<StocksRepository>(options =>
            options.UseSqlServer(defaultConnectionString));

        services.AddScoped<IStocksService, StocksService>();

        return builder;
    }
    
    public static WebApplicationBuilder AddCompaniesConfiguration(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        
        var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<CompaniesRepository>(options =>
            options.UseSqlServer(defaultConnectionString));

        services.AddScoped<ICompaniesService, CompaniesService>();
        services.AddScoped<ICompanyUsersService, CompanyUsersService>();

        return builder;
    }
}