using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace VSMS.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    [ForeignKey(nameof(Company.Id))]
    public Guid CompanyId { get; set; }
    public Company Company { get; set; }
}