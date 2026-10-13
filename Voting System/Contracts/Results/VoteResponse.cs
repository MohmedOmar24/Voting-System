using Microsoft.Identity.Client;

namespace Voting_System.Contracts.Results;

public record VoteResponses
(
    string Title,
    IEnumerable<VoteSubmissionResponse> VoteSubmissions
    );
