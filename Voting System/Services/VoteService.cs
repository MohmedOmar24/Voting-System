
using System.Threading;
using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Voting_System.Contracts.Votes;
using Voting_System.Entities;
using Voting_System.Errors;
using Voting_System.Persistence;

namespace Voting_System.Services;

public class VoteService(ApplicationDbContext context,
                         INotificationService notificationService) : IVoteService
{
    private readonly ApplicationDbContext _context=context;
    private readonly INotificationService _notificationService= notificationService; 
  
    public async Task<Result<VoteResponse>> GetAsync([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var vote = await _context.Votes.FindAsync(id, cancellationToken);

        if (vote is null)
            return Result.Failure<VoteResponse>(VoteErrors.VoteNotFound);
        else
            return Result.Success(vote.Adapt<VoteResponse>());
    } // =>await _context.Votes.FindAsync(id,cancellationToken) ;

    public async Task<IEnumerable<VoteResponse>> GetAllAsync(CancellationToken cancellationToken = default)
               => await _context.Votes
                        .AsNoTracking()
                        .ProjectToType<VoteResponse>()
                        .ToListAsync(cancellationToken);

    public async Task<IEnumerable<VoteResponse>> GetCurrentAsync(CancellationToken cancellationToken = default)
              => await _context.Votes
                       .Where(      
                           x=>x.IsPublished
                        && x.StartsAt<=DateOnly.FromDateTime(DateTime.UtcNow)
                        && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow)
                           )
                       .AsNoTracking()
                       .ProjectToType<VoteResponse>()
                       .ToListAsync(cancellationToken);

    public async Task<Result<VoteResponse>> AddAsync(VoteRequest voterequest, CancellationToken cancellationToken=default)
    {
        var isExistedVote= await _context.Votes.AnyAsync(v=>v.Title==voterequest.Title,cancellationToken);

        if(isExistedVote)
            return Result.Failure<VoteResponse>(VoteErrors.DuplicatedVoteTitle);
        var vote= voterequest.Adapt<Vote>();
       await _context.AddAsync(vote,cancellationToken);
     
       await _context.SaveChangesAsync(cancellationToken) ;
       
        var voteResponse = vote.Adapt<VoteResponse>();
        return Result.Success(voteResponse);
    }

    public async Task<Result> UpdateAsync(int id, VoteRequest voteRequest, CancellationToken cancellationToken = default)

    {
        var isExistedVote = await _context.Votes.AnyAsync(v => v.Title == voteRequest.Title&&v.Id!=id, cancellationToken);

        if (isExistedVote)
            return Result.Failure<VoteResponse>(VoteErrors.DuplicatedVoteTitle);

        var currentvote = await _context.Votes.FindAsync(id, cancellationToken);

        if (currentvote is null)
            return Result.Failure(VoteErrors.VoteNotFound);

        currentvote.Title = voteRequest.Title;
        currentvote.Summary = voteRequest.Summary;
        currentvote.StartsAt = voteRequest.StartsAt;
        currentvote.EndsAt = voteRequest.EndsAt;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }



    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var vote = await _context.Votes.FindAsync(id, cancellationToken);
        if (vote is null)
            return Result.Failure(VoteErrors.VoteNotFound);
        _context.Remove(vote);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }



    public async Task<Result> TogglePublishAsync(int id, CancellationToken cancellationToken = default)
    {
        var vote = await _context.Votes.FindAsync(id, cancellationToken);

        if (vote is null)
            return Result.Failure(VoteErrors.VoteNotFound);

       
        vote.IsPublished=!vote.IsPublished;

        await _context.SaveChangesAsync(cancellationToken);

        if (vote.IsPublished && vote.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
            BackgroundJob.Enqueue(() => _notificationService.SendNewVoteNotification(id));
        return Result.Success();
    }

}
