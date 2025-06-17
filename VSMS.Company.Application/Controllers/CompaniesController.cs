using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VSMS.Company.Infrastructure.Interfaces;

namespace VSMS.Company.Application.Controllers;

[ApiController]
[Route("[controller]")]
public class CompaniesController(
    ILogger<CompaniesController> logger,
    ICompaniesService companiesService) : ControllerBase
{
    
}