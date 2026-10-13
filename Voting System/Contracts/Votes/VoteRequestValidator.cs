using FluentValidation;

namespace Voting_System.Contracts.Votes;

public class VoteRequestValidator : AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
    }
}
