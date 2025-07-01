using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using VSMS.Application.Swagger;
using VSMS.Domain.Entities;
using VSMS.Infrastructure.Extensions;
using VSMS.Infrastructure.Hubs;
using VSMS.Infrastructure.Initializers;

namespace VSMS.Application;

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

        Log.Warning("Starting web host");
        try
        {
            builder.AddSwaggerConfiguration();
            
            builder.AddIdentityConfiguration();
            builder.AddCompaniesConfiguration();
            builder.AddStocksConfiguration();
            builder.AddApplicationConfiguration();
            builder.AddSimulationConfiguration();
            
            builder.Services.AddCors(options =>
                {
                    options.AddDefaultPolicy(policy =>
                        {
                            policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        }
                    );
                }
            );
            builder.Services.AddControllers();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDataProtection();
            
            builder.Services.AddSignalR();

            builder.Services.AddAntiforgery(options => options.SuppressXFrameOptionsHeader = true);
            
            var app = builder.Build();
            
            using (var scope = app.Services.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                await RoleInitializer.Initialize(roleManager, logger);
                
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                await UserInitializer.Initialize(userManager, logger);
            }

            app.UseStaticFiles();
            
            app.UseSwagger();
            app.UseSwaggerUI();
            
            app.UseCors();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            app.UseForwardedHeaders(new()
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ApplicationHub>("ApplicationHub");
                endpoints.MapHub<StocksHub>("StocksHub");
            });
            
            app.Run();
        }
        catch (Exception e)
        {
            Log.Fatal(e, e.Message);
        }
        finally
        {
            Log.Warning("Web host shutdown");
            Log.CloseAndFlush();
        }
    }
}