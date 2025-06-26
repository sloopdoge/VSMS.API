using Microsoft.EntityFrameworkCore;
using VSMS.Domain.Entities;

namespace VSMS.Repository;

public class CompaniesRepository(
    DbContextOptions<CompaniesRepository> options) : DbContext(options)
{
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Prevent EF Core from generating tables for user and stock entities
        modelBuilder.Ignore<ApplicationUser>();
        modelBuilder.Ignore<Stock>();

        // Ignore navigation properties that belong to other contexts
        modelBuilder.Entity<Company>().Ignore(c => c.Users);
        modelBuilder.Entity<Company>().Ignore(c => c.Stocks);
    }
}