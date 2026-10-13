namespace Voting_System.Contracts.Roles;

public record RoleRequest
(
    string Name,
    IList<string> Permissions
);
