using FluentValidation;

using Voting_System.Abstractions.Consts;

namespace Voting_System.Contracts.Authentication;


public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
                            .NotEmpty()
                            .EmailAddress();

        RuleFor(x => x.NewPassword)
                            .NotEmpty()
                            .Matches(RegexPatterns.Password)
                            .WithMessage
   ("Password must be at least 8 digits and should contain Lowercase,Nonalphanumeric and Uppercase");


        RuleFor(x => x.Code)
             .NotEmpty();
            

    }
}