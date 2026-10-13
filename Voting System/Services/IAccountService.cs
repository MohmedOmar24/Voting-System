using Voting_System.Contracts.Users;

namespace Voting_System.Services;

public interface IAccountService
{
    Task<Result<UserProfileResponse>> GetProfileAsync(string userId);

    Task<Result> UpdateProfileAsync(string userId,UpdateProfileRequest request);    

    Task<Result> ChangePasswordAsync(string userId,ChangePasswordRequest request);

}
