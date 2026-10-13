using Voting_System.Contracts.Authentication;

namespace Voting_System.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string Email,string Password ,CancellationToken cancellationToken=default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default);
    Task<Result> RevokeRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default);

    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request);

    Task<Result> SendResetPasswordCodeAsync(string email);

    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);


}
