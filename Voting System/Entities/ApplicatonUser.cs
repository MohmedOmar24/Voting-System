using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Voting_System.Entities;

public sealed class ApplicatonUser :IdentityUser
{
    public string FirstName { get; set; } =string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];

}
