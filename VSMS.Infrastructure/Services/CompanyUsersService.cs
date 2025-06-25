using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VSMS.Domain.DTOs;
using VSMS.Domain.Entities;
using VSMS.Domain.Exceptions;
using VSMS.Infrastructure.Interfaces;
using VSMS.Repository;

namespace VSMS.Infrastructure.Services;

public class CompanyUsersService(
    ILogger<CompanyUsersService> logger,
    CompaniesDbContext companiesContext,
    UserManager<ApplicationUser> userManager) : ICompanyUsersService
{
    public async Task<bool> AssignUserToCompany(Guid userId, Guid companyId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new UserNotFoundException(userId);

            var company = await companiesContext.Companies.FindAsync(companyId);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            user.CompanyId = companyId;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(Environment.NewLine, result.Errors));

            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<bool> UnassignUserFromCompany(Guid userId, Guid companyId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new UserNotFoundException(userId);

            var company = await companiesContext.Companies.FindAsync(companyId);
            if (company is null)
                throw new CompanyNotFoundException(companyId);

            if (user.CompanyId != companyId)
                return false;

            user.CompanyId = Guid.Empty;
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(Environment.NewLine, result.Errors));

            return true;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }

    public async Task<List<UserProfileDto>> GetAllUsersInCompany(Guid companyId)
    {
        try
        {
            var company = await companiesContext.Companies
                .Include(c => c.Users)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company is null)
                throw new CompanyNotFoundException(companyId);

            var profiles = await Task.WhenAll(company.Users.Select(async user =>
            {
                var roles = await userManager.GetRolesAsync(user);
                return new UserProfileDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = roles.FirstOrDefault() ?? "None"
                };
            }));

            return profiles.ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message, e);
        }
    }
}