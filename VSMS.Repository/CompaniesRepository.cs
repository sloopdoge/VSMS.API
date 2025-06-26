using Microsoft.EntityFrameworkCore;
using VSMS.Domain.Entities;

namespace VSMS.Repository;

public class CompaniesRepository(
    DbContextOptions<CompaniesRepository> options) : DbContext(options)
{
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Stock>();
        modelBuilder.Ignore<ApplicationUser>();

        modelBuilder.Entity<Company>()
            .HasMany(c => c.Users)
            .WithOne()
            .HasForeignKey(u => u.CompanyId);

        modelBuilder.Entity<Company>()
            .HasMany(c => c.Stocks)
            .WithOne(s => s.Company)
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}