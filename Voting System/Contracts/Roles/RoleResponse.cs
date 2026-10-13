using System.Diagnostics.Eventing.Reader;

namespace Voting_System.Contracts.Roles;

public record RoleResponse
(
    string Id,
    string Name,
    bool IsDeleted
);