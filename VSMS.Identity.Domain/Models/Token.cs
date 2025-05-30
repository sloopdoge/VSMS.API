namespace VSMS.Identity.Domain.Models;

public class Token
{
    public required string Value { get; set; }
    public DateTime Expires { get; set; }
}