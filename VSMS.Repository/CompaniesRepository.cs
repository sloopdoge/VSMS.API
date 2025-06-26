using Microsoft.EntityFrameworkCore;
using VSMS.Domain.Entities;

namespace VSMS.Repository;

public class CompaniesRepository(
    DbContextOptions<CompaniesRepository> options) : DbContext(options)
{
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<ApplicationUser>();
        modelBuilder.Ignore<Stock>();

        modelBuilder.Entity<Company>().Ignore(c => c.Users);
        modelBuilder.Entity<Company>().Ignore(c => c.Stocks);
    }
}