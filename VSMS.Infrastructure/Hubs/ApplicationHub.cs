using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace VSMS.Infrastructure.Hubs;

[Authorize]
public class ApplicationHub(
    ILogger<ApplicationHub> logger) 
    : BaseHub(logger)
{
    
}