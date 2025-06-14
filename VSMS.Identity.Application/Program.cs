using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using VSMS.Identity.Domain.Entities;
using VSMS.Identity.Infrastructure.Initializers;
using VSMS.Identity.Infrastructure.Interfaces;
using VSMS.Identity.Infrastructure.Services;
using VSMS.Identity.Repository;

namespace VSMS.Identity.Application;

public abstract class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region Serilog Logger

        if (builder.Environment.IsDevelopment())
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .CreateLogger();
        }

        if (builder.Environment.IsProduction())
        {
            var lokiUri = builder.Configuration.GetValue<string>("LokiSettings:Url");
            var appName = builder.Configuration.GetValue<string>("LokiSettings:AppName");

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.GrafanaLoki(
                    lokiUri,
                    labels: new[]
                    {
                        new LokiLabel
                        {
                            Key = "app",
                            Value = appName
                        }
                    },
                    propertiesAsLabels: new[] { "app" })
                .CreateLogger();
        }

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog();

        #endregion

        Log.Warning("Starting Identity Service web host");
        try
        {
            // Add services to the container.
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options => 
                options.UseSqlServer(defaultConnectionString));
            
            builder.Services.AddIdentityCore<ApplicationUser>()
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            
            builder.Services.AddAuthentication(options =>
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
                    //
                    // options.Events = new()
                    // {
                    //     OnMessageReceived = context =>
                    //     {
                    //         var accessToken = context.Request.Query["access_token"];
                    //
                    //         var path = context.HttpContext.Request.Path;
                    //         if (!string.IsNullOrEmpty(accessToken) &&
                    //             (path.StartsWithSegments("/hubs")))
                    //         {
                    //             context.Token = accessToken;
                    //         }
                    //         return Task.CompletedTask;
                    //     },
                    //     OnChallenge = context =>
                    //     {
                    //         var accessToken = context.Request.Query["access_token"];
                    //         Log.Debug("Access Token: {accessToken}", accessToken);
                    //         return Task.CompletedTask;
                    //     },
                    //     OnAuthenticationFailed = context =>
                    //     {
                    //         Log.Error("Authentication failed: {Exception}", context.Exception);
                    //         return Task.CompletedTask;
                    //     },
                    //     OnTokenValidated = context =>
                    //     {
                    //         Log.Debug("Token validated successfully for user: {ConnectionId}", context.HttpContext.Connection.Id);
                    //         return Task.CompletedTask;
                    //     },
                    // };
                    //
                    // options.IncludeErrorDetails = true;
                });
            
            builder.Services.AddAuthorization();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            
            builder.Services.AddControllers();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                await RoleInitializer.Initialize(roleManager, logger);
                
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                await UserInitializer.Initialize(userManager, logger);
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/Identity/swagger/v1/swagger.json", "Identity Service API");
                c.RoutePrefix = "api/Identity/swagger";
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            
            app.Run();
        }
        catch (Exception e)
        {
            Log.Fatal(e, e.Message);
        }
        finally
        {
            Log.Warning("Identity Service web host shutdown");
            await Log.CloseAndFlushAsync();
        }
    }
}