using FluentValidation.AspNetCore;

namespace Voting_System.Contracts.VoteSubmissions;

public class VoteSubmissionRequestValidator :AbstractValidator<VoteSubmissionRequest>
{
    public VoteSubmissionRequestValidator()
    {
        RuleFor(x => x.Answers).NotEmpty();

        RuleForEach(x => x.Answers)
            .SetInheritanceValidator(v => v.Add(new VoteSubmissionAnswerRequestValidator()));
    }
}
