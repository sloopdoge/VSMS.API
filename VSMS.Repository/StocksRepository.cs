using Microsoft.EntityFrameworkCore;
using VSMS.Domain.Entities;

namespace VSMS.Repository;

public class StocksRepository(
    DbContextOptions<StocksRepository> options) : DbContext(options)
{
    public DbSet<Stock> Stocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ApplicationUser>();
        modelBuilder.Ignore<Company>();

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