using Voting_System.Contracts.Questions;
namespace Voting_System.Services;

public interface IQuestionService
{
    Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int voteId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<QuestionResponse>>> GetAvalaibleAsync(int voteId,string? userid, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> GetAsync(int voteId,int id, CancellationToken cancellationToken = default);
    Task<Result<QuestionResponse>> AddAsync(int voteId,QuestionRequest request,CancellationToken cancellationToken=default);
    Task<Result> UpdateAsync(int voteId, int id, QuestionRequest request, CancellationToken cancellationToken = default);
    Task<Result> ToggleStatusAsync(int voteId,int id, CancellationToken cancellationToken = default);
}
