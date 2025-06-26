using Microsoft.AspNetCore.Identity;

namespace VSMS.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public Guid? CompanyId { get; set; }
}