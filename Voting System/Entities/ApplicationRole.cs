using Microsoft.AspNetCore.Identity;


namespace Voting_System.Entities;

public class ApplicationRole :IdentityRole
{
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
}
