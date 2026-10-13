namespace Voting_System.Errors;

public static class VoteErrors
{
    public static readonly Error VoteNotFound = 
                    new("Vote.NotFound", "no vote was found with the given Id",StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedVoteTitle =
                    new("vote.DuplicatedVoteTitle", "there is exist vote with the same title", StatusCodes.Status409Conflict);
}
