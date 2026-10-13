using Microsoft.AspNetCore.Authorization;

namespace Voting_System.Authentication.Filters;

public class PermissionRequirement(string permission):IAuthorizationRequirement
{
    public string Permission { get;  }=permission;
}
