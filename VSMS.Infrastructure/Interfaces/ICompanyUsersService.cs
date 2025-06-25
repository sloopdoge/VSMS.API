using VSMS.Domain.DTOs;

namespace VSMS.Infrastructure.Interfaces;

public interface ICompanyUsersService
{
    Task<bool> AssignUserToCompany(Guid userId, Guid companyId);
    Task<bool> UnassignUserFromCompany(Guid userId, Guid companyId);
    Task<List<UserProfileDto>> GetAllUsersInCompany(Guid companyId);
}