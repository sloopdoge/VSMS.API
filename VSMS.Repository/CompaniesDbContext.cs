using Microsoft.EntityFrameworkCore;

namespace VSMS.Repository;

public class CompaniesDbContext(
    DbContextOptions<CompaniesDbContext> options) : DbContext(options)
{
    public DbSet<Domain.Entities.Company> Companies { get; set; }
}