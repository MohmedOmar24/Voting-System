using Voting_System.Contracts.VoteSubmissions;

namespace Voting_System.Services;

public interface IVoteSubmissionService
{
    Task<Result> AddAsync(int voteId, string userId, VoteSubmissionRequest request, CancellationToken cancellationToken = default);
}
