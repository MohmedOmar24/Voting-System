namespace Voting_System.Errors;

public class VoteSubmissionErrors
{
    // public static readonly Error VoteNotFound =
    //               new("Vote.NotFound", "no vote was found with the given Id");

    public static readonly Error DuplicatedVoteSubmission =
                    new("VoteSubmission.DuplicatedVoteSubmission", "this vote has been submited before by the current user.", StatusCodes.Status409Conflict);
    public static readonly Error InvalidQuestions =
                new("VoteSubmission.InvalidQuestions", "invalid questions", StatusCodes.Status400BadRequest);
}
