using FluentValidation;

using Voting_System.Abstractions.Consts;
namespace Voting_System.Contracts.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
                            .NotEmpty()
                            .EmailAddress();

        RuleFor(x => x.Password)
                            .NotEmpty()
                            .Matches(RegexPatterns.Password)
                            .WithMessage
   ("Password must be at least 8 digits and should contain Lowercase,Nonalphanumeric and Uppercase");


        RuleFor(x => x.FirstName)
                            .NotEmpty()
                            .Length(3,100);
     
        RuleFor(x => x.LastName)
                            .NotEmpty()
                            .Length(3, 100);


    }
}
