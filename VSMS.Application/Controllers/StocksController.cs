using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VSMS.Infrastructure.Interfaces;

namespace VSMS.Application.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class StocksController(
    ILogger<StocksController> logger,
    IStocksService stocksService) : ControllerBase
{
    
}