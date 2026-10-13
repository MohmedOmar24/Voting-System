using Mapster;
using Microsoft.EntityFrameworkCore;
using Voting_System.Contracts.Questions;
using Voting_System.Contracts.VoteSubmissions;
using Voting_System.Errors;
using Voting_System.Persistence;

namespace Voting_System.Services;

public class VoteSubmissionService(ApplicationDbContext dbContext):IVoteSubmissionService
{
    private readonly ApplicationDbContext _dbContext=dbContext;
    public async Task<Result> AddAsync(int voteId, string userId, VoteSubmissionRequest request, CancellationToken cancellationToken = default)
    {

        var hasSubmitVote = await _dbContext.VoteSubmissions.AnyAsync(v => v.UserId == userId && v.VoteId == voteId, cancellationToken);

        if (hasSubmitVote)
            return Result.Failure(VoteSubmissionErrors.DuplicatedVoteSubmission);

        var voteExist = await _dbContext.Votes.AnyAsync(x => x.Id == voteId
                                                   && x.IsPublished
                                                   && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow)
                                                   && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        if (!voteExist)
            return Result.Failure(VoteErrors.VoteNotFound);

        var AvaliableQuestions =await _dbContext.Questions
                       .Where(x => x.VoteId == voteId&&x.IsActive)
                       .Select(x => x.VoteId)
                       .ToListAsync(cancellationToken);

        if (request.Answers.Select(x => x.QuestionId).SequenceEqual(AvaliableQuestions))
            return Result.Failure(VoteSubmissionErrors.InvalidQuestions);

        var voteSubmission = new VoteSubmission
        {
            UserId = userId,
            VoteId = voteId,
            VoteSubmissionAnswers = request.Answers.Adapt<IEnumerable<VoteSubmissionAnswer>>().ToList()
        };
       await _dbContext.AddAsync(voteSubmission,cancellationToken);
       await _dbContext.SaveChangesAsync(cancellationToken);


        return Result.Success();
    }

}
