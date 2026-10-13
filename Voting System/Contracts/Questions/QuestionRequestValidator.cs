namespace Voting_System.Contracts.Questions;

public class QuestionRequestValidator: AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(3, 1000);

        RuleFor(x => x.Answers)
            .NotNull()
            .Must(x=>x.Count > 1)
            .WithMessage("question must have at least two answers");

        RuleFor(x => x.Answers)
            .NotNull()
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("not allowed repeted answers");
            
    }
    
}
