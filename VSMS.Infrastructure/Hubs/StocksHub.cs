using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace VSMS.Infrastructure.Hubs;

[Authorize]
public class StocksHub(
    ILogger<StocksHub> logger) 
    : BaseHub(logger)
{
    
}