using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Org.BouncyCastle.Asn1;
using Voting_System.Helpers;
using Voting_System.Persistence;

namespace Voting_System.Services;

public class NotificationService(
                             ApplicationDbContext dbContext ,
                             UserManager<ApplicatonUser> userManager,
                             IHttpContextAccessor httpContextAccessor,
                             IEmailSender emailSender
                                    ): INotificationService
{
    private readonly UserManager<ApplicatonUser> _userManager=userManager;
    private readonly IEmailSender _emailSender=emailSender;
    private readonly ApplicationDbContext _dbcontext=dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor=httpContextAccessor;
   public async Task SendNewVoteNotification(int? voteId = null)
    {
        IEnumerable<Vote>  votes=[];
        if(voteId.HasValue)
        {
            var vote =await _dbcontext.Votes.SingleOrDefaultAsync(v => v.Id == voteId && v.IsPublished);
            votes = [vote!];
        }
        else
        {
            votes = await _dbcontext.Votes
                .Where(v => v.IsPublished && v.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                .AsNoTracking()
                .ToListAsync();
        }

        var users= await _userManager.Users.ToListAsync();

        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
        foreach (var vote in votes)
        {
            foreach(var user in users)
            {
                var placeholder = new Dictionary<string, string>
                {
                   {"{{name}}",user.FirstName },
                   {"{{voteTill}}",vote.Title },
                   {"{{endDate}}",vote.EndsAt.ToString() },
                   {"{{url}}",$"{origin}/votes/start/{vote.Id}" }
                };

                var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholder);
                await _emailSender.SendEmailAsync(user.Email!,"VotingSystem: new vote", body);
            }
        }

    }

}
