using Voting_System.Contracts.Answers;

namespace Voting_System.Contracts.Questions;

public record QuestionResponse(
    int Id,
    string Content,
    IEnumerable<AnswerResponse> Answers
    );

