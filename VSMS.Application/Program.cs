using FluentValidation;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

namespace VSMS.Application;

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

        Log.Warning("Starting web host");
        try
        {
            // Add services to the container.
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            builder.Services.AddAuthorization();

            builder.Services.AddControllers();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();

            var app = builder.Build();

            app.UseStaticFiles();
            
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/identity/swagger/v1/swagger.json", "Identity Service API");
                c.SwaggerEndpoint("/api/companies/swagger/v1/swagger.json", "Company Service API");
                c.SwaggerEndpoint("/api/stocks/swagger/v1/swagger.json", "Stock Service API");
                c.RoutePrefix = "api/swagger";
            });

            app.MapControllers();

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