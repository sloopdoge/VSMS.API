using Microsoft.EntityFrameworkCore;
using VSMS.Domain.Entities;

namespace VSMS.Repository;

public class CompaniesDbContext(
    DbContextOptions<CompaniesDbContext> options) : DbContext(options)
{
    public DbSet<Company> Companies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>()
            .HasMany(c => c.Users)
            .WithOne(u => u.Company);
    }
}