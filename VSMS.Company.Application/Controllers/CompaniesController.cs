using Microsoft.AspNetCore.Mvc;

namespace VSMS.Company.Application.Controllers;

[ApiController]
[Route("")]
public class CompaniesController(
    ILogger<CompaniesController> logger) : ControllerBase
{
    
}