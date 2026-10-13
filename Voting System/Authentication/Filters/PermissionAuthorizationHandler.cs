using Microsoft.AspNetCore.Authorization;
using Voting_System.Abstractions.Consts;

namespace Voting_System.Authentication.Filters;

public class PermissionAuthorizationHandler:AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,PermissionRequirement Requirement)
    {
        if (context.User.Identity is not { IsAuthenticated: true } ||
            !context.User.Claims.Any(x => x.Value == Requirement.Permission && x.Type == Permissions.Type)
            )
              return;

        context.Succeed(Requirement);
        return;
    }
}
