using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VSMS.Domain.Entities;

namespace VSMS.Repository;

public class ApplicationRepository(DbContextOptions<ApplicationRepository> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    
}