using Voting_System.Abstractions.Consts;

namespace Voting_System.Contracts.Users;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {


        RuleFor(x => x.CurrentPassword)
                            .NotEmpty();

        RuleFor(x => x.NewPassword)
                          .NotEmpty()
                          .Matches(RegexPatterns.Password)
                          .WithMessage
     ("Password must be at least 8 digits and should contain Lowercase,Nonalphanumeric and Uppercase")
                          .NotEqual(x => x.CurrentPassword)
                          .WithMessage
                          ("new password should not be the same as the current password");


    }
}
