using Mapster;
using Voting_System.Contracts.Answers;
using Voting_System.Contracts.Questions;
using Voting_System.Errors;
using Voting_System.Persistence;

namespace Voting_System.Services;

public class QuestionService(ApplicationDbContext dbContext) : IQuestionService
{
    private readonly ApplicationDbContext _dbContext=dbContext;

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int voteId, CancellationToken cancellationToken = default)
    {
        var voteIsExist = await _dbContext.Votes.AnyAsync(v => v.Id == voteId, cancellationToken);
        if (!voteIsExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.VoteNotFound);

        var questions= await _dbContext.Questions.
                      Where(x=>x.VoteId==voteId)
                      .Include(x=>x.Answers)
                     /* .Select(q=>new QuestionResponse
                      (
                          q.Id,
                          q.Content,
                          q.Answers.Select(a=>new AnswerResponse(a.Id, a.Content))         
                      ))*/
                     .ProjectToType<QuestionResponse>()
                      .AsNoTracking()
                      .ToListAsync(cancellationToken);
       

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }

    public async Task<Result<IEnumerable<QuestionResponse>>> GetAvalaibleAsync(int voteId, string? userid, CancellationToken cancellationToken = default)
    {
        var hasSubmitVote= await _dbContext.VoteSubmissions.AnyAsync(v=>v.UserId==userid&&v.VoteId==voteId, cancellationToken);

        if (hasSubmitVote)
            return Result.Failure< IEnumerable<QuestionResponse>>(VoteSubmissionErrors.DuplicatedVoteSubmission);

        var voteExist=await _dbContext.Votes.AnyAsync(x=>x.Id==voteId
                                                   && x.IsPublished
                                                   && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                                                   && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);  

        if(!voteExist)
            return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.VoteNotFound);

        var questions= await _dbContext.Questions.Where(x=>x.VoteId==voteId&&x.IsActive)
            .Include(x=>x.Answers)
            .Select( q=> new QuestionResponse
            (
                q.Id,q.Content,
                q.Answers.Where(a=>a.IsActive).Select(a=>new AnswerResponse(a.Id,a.Content))
            )
            )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<QuestionResponse>>(questions);
    }


    public async Task<Result<QuestionResponse>> GetAsync(int voteId, int id, CancellationToken cancellationToken = default)
    {
        var question = await _dbContext.Questions.
                      Where(x => x.VoteId == voteId && x.Id == id)
                      .Include(x => x.Answers)
                     .ProjectToType<QuestionResponse>()
                      .AsNoTracking()
                      .SingleOrDefaultAsync(cancellationToken);

        return question is null?
            Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound) 
            :Result.Success(question);
    }
    public async Task<Result<QuestionResponse>> AddAsync(int voteId, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var voteIsExist=await _dbContext.Votes.AnyAsync(v=>v.Id==voteId,cancellationToken);
        if(!voteIsExist)
            return Result.Failure<QuestionResponse>(VoteErrors.VoteNotFound);

        var QuesIsExist=await _dbContext.Questions.AnyAsync(q=>q.Content==request.Content&&voteId==q.VoteId,cancellationToken);

        if(QuesIsExist)
            return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

        var question = request.Adapt<Question>();
        question.VoteId = voteId;

       // request.Answers.ForEach(answer => question.Answers.Add(new Answer { Content=answer}));

        await _dbContext.AddAsync(question,cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(question.Adapt<QuestionResponse>());
    }

    public async Task<Result> UpdateAsync(int voteId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
    {
        var questionIsExist = await _dbContext.Questions
                                 .AnyAsync(x => x.VoteId == voteId && x.Id != id && x.Content == request.Content,cancellationToken);
        if (questionIsExist)
            return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

        var question = await _dbContext.Questions.Include(x => x.Answers)
            .SingleOrDefaultAsync(x => x.VoteId == voteId && x.Id != id, cancellationToken);

        if (question is null)
           return Result.Failure(QuestionErrors.QuestionNotFound);

        question.Content = request.Content;

        var currentAnswers= question.Answers.Select(x => x.Content).ToList();

        var newAnswers=request.Answers.ToList().Except(currentAnswers).ToList();

        newAnswers.ForEach(answer=> question.Answers.Add(new Answer { Content = answer }));

        question.Answers.ToList().ForEach(answer=> answer.IsActive=request.Answers.Contains(answer.Content));
      
        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }

    public async Task<Result> ToggleStatusAsync(int voteId,int id, CancellationToken cancellationToken = default)
    {
        var question = await _dbContext.Questions.SingleOrDefaultAsync(x => x.Id == id && x.VoteId == voteId, cancellationToken);

        if (question is null)
            return Result.Failure(QuestionErrors.QuestionNotFound);


        question.IsActive = !question.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }
}
