namespace Voting_System.Errors;

public static class QuestionErrors
{
    public static readonly Error QuestionNotFound = 
                    new("Question.NotFound", "no question was found with the given Id", StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedQuestionContent =
                    new("Question.DuplicatedQuestionContent", "there is exist a question with the same content within this vote", StatusCodes.Status409Conflict);
}
