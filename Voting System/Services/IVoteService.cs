using Voting_System.Contracts.Votes;

namespace Voting_System.Services;

public interface IVoteService
{
     Task<IEnumerable <VoteResponse>> GetAllAsync( CancellationToken cancellationToken = default);
    Task<IEnumerable<VoteResponse>> GetCurrentAsync(CancellationToken cancellationToken = default);

    Task<Result<VoteResponse>> GetAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<VoteResponse>> AddAsync(VoteRequest vote, CancellationToken cancellationToken =default);
    Task<Result> UpdateAsync(int id, VoteRequest voteRequest, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result> TogglePublishAsync(int id, CancellationToken cancellationToken = default);

}
