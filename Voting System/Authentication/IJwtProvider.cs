namespace Voting_System.Authentication;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(ApplicatonUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
 // (string token, int expiresin) GenerateToken(ApplicatonUser user, IEnumerable<string> roles, IEnumerable<string> permissions)

    string? ValidateToken(string token);
}                                                                                                                                                                                                                                                                