using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using VSMS.Domain.Constants;
using VSMS.Domain.Entities;
using VSMS.Domain.Exceptions;

namespace VSMS.Infrastructure.Identity;

public class CompanyOwnershipHandler(UserManager<ApplicationUser> userManager) : AuthorizationHandler<CompanyOwnershipRequirement, Guid>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, 
        CompanyOwnershipRequirement requirement, Guid resource) 
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return;

        var user = await userManager.FindByIdAsync(userId);

        if (user is null)
            throw new UserNotFoundException(userId);

        if (await userManager.IsInRoleAsync(user, RoleNames.Admin))
        {
            context.Succeed(requirement);
            return;
        }

        if (user.CompanyId.HasValue && resource == user.CompanyId)
        {
            context.Succeed(requirement);
        }
    }
}