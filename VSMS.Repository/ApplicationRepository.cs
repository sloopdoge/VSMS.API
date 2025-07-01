using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VSMS.Domain.Entities;

namespace VSMS.Repository;

public class ApplicationRepository(DbContextOptions<ApplicationRepository> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<Stock> Stocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Company)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.CompanyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Company>()
            .HasMany(c => c.Stocks)
            .WithOne(s => s.Company)
            .HasForeignKey(s => s.CompanyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Company>()
            .HasMany(c => c.Users)
            .WithOne(u => u.Company)
            .HasForeignKey(u => u.CompanyId)
            .IsRequired(false);
        
        modelBuilder.Entity<Stock>()
            .HasOne(s => s.Company)
            .WithMany(c => c.Stocks)
            .HasForeignKey(s => s.CompanyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.Property(s => s.Price).HasPrecision(18, 6);
        });
        
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
        
        base.OnModelCreating(modelBuilder);
    }
}