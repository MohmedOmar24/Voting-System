using System.ComponentModel.DataAnnotations;

namespace Voting_System.Authentication;

public class JwtOptions
{
    public static string SectionName = "Jwt";
    [Required]
    public string Key { get; init; } = string.Empty;
    [Required]
    public string issuer { get; init; } = string.Empty;
    [Required]
    public string audience { get; init; } = string.Empty;
    [Range(1,int.MaxValue)]
    public int expiryin { get; init; }

}
