using Microsoft.EntityFrameworkCore;
using VSMS.Domain.Entities;

namespace VSMS.Repository;

public class StocksRepository(
    DbContextOptions<StocksRepository> options) : DbContext(options)
{
    public DbSet<Stock> Stocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Exclude unrelated entities from this context
        modelBuilder.Ignore<ApplicationUser>();
        modelBuilder.Ignore<Company>();

        // Ignore navigation properties that point to other contexts
        modelBuilder.Entity<Stock>().Ignore(s => s.Company);

        modelBuilder.Entity<Stock>()
            .ToTable(
                "Stocks",
                s => s.IsTemporal(t =>
                {
                    t.HasPeriodStart("PeriodStart");
                    t.HasPeriodEnd("PeriodEnd");
                    t.UseHistoryTable("StocksHistoricalData");
                }));

        modelBuilder.Entity<Stock>().Property<DateTime>("PeriodStart");
        modelBuilder.Entity<Stock>().Property<DateTime>("PeriodEnd");
    }
}