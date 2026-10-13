namespace Voting_System.Contracts.Results;

public record VoteSubmissionResponse
(
    string VoterName,
    DateTime VoteSubmitDate,
    IEnumerable<QuestionAnswerResponse> SelectedAnswers
    );
