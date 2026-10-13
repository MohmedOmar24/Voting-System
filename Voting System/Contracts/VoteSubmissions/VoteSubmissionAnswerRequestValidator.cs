namespace Voting_System.Contracts.VoteSubmissions;

public class VoteSubmissionAnswerRequestValidator : AbstractValidator<VoteSubmissionAnswerRequest>
{
    public VoteSubmissionAnswerRequestValidator()
    {
        RuleFor(x => x.QuestionId).GreaterThan(0);
        RuleFor(x => x.AnswerId).GreaterThan(0);

    }
}