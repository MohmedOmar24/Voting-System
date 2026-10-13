
namespace Voting_System.Errors;

public static class RoleErrors
{
    public static readonly Error RoleNotFound = 
        new("Role.RoleNotFound", "Role is not found.", StatusCodes.Status404NotFound);
   
    public static readonly Error DuplicatedRoleName =
        new("Role.DuplicatedRoleName", "there is exist role with the same name", StatusCodes.Status409Conflict);
    
    public static readonly Error InvalidPermission =
        new("Role.InvalidPermission", "Inavalid permissions", StatusCodes.Status400BadRequest);
}
