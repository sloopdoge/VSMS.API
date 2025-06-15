using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Grafana.Loki;
using VSMS.Company.Infrastructure.Interfaces;
using VSMS.Company.Infrastructure.Services;
using VSMS.Company.Repository;

namespace VSMS.Company.Application;

public abstract class Program
{
    public static void Main(string[] args)
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

        Log.Warning("Starting Company Service web host");
        try
        {
            // Add services to the container.
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            
            var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<CompaniesDbContext>(options => 
                options.UseSqlServer(defaultConnectionString));

            builder.Services.AddScoped<ICompaniesService, CompaniesService>();

            var app = builder.Build();
            
            app.UsePathBase("/api/Companies");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/Companies/swagger/v1/swagger.json", "Company Service API V1");
                c.RoutePrefix = "api/Companies/swagger";
            });

            app.UseHttpsRedirection();
            
            app.MapControllers();

            app.Run();
        }
        catch (Exception e)
        {
            Log.Fatal(e, e.Message);
        }
        finally
        {
            Log.Warning("Company Service web host shutdown");
            Log.CloseAndFlush();
        }
    }
}