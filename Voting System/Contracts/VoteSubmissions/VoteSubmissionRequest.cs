namespace Voting_System.Contracts.VoteSubmissions;

public class VoteSubmissionRequest
{
    public IEnumerable<VoteSubmissionAnswerRequest> Answers { get; set; } = [];
}
