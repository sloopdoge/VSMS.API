using Microsoft.Extensions.Logging;
using VSMS.Domain.DTOs;
using VSMS.Infrastructure.Interfaces;

namespace VSMS.Infrastructure.Services;

public class CompanyUsersService(
    ILogger<CompanyUsersService> logger) : ICompanyUsersService
{
    public async Task<bool> AssignUserToCompany(Guid userId, Guid companyId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UnassignUserFromCompany(Guid userId, Guid companyId)
    {
        throw new NotImplementedException();
    }

    public async Task<List<UserProfileDto>> GetAllUsersInCompany(Guid companyId)
    {
        throw new NotImplementedException();
    }
}