using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VSMS.Infrastructure.Services.Engines;
using VSMS.Repository;

namespace VSMS.Infrastructure.Services.Background;

public class StockSimulationService(
    IServiceScopeFactory scopeFactory, 
    ILogger<StockSimulationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var random = new Random();
        var engine = new StockSimulationEngine();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationRepository>();
                 var stocks = await db.Stocks.ToListAsync(stoppingToken);

                foreach (var stock in stocks)
                {
                    engine.UpdateStockState(stock);
                    engine.UpdateStockPrice(stock);
                }

                await db.SaveChangesAsync(stoppingToken);
                logger.LogInformation("Updated {Count} stocks at {Time}", stocks.Count, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Stock simulation failed");
            }

            var waitTime = TimeSpan.FromSeconds(random.NextInt64(1, 360));
            await Task.Delay(waitTime, stoppingToken);
        }
    }
}