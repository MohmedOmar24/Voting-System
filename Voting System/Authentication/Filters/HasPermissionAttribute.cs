using Microsoft.AspNetCore.Authorization;

namespace Voting_System.Authentication.Filters;

public class HasPermissionAttribute(string permission):AuthorizeAttribute(permission)
{

}
