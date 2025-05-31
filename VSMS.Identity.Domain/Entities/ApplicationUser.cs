using Microsoft.AspNetCore.Identity;

namespace VSMS.Identity.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}