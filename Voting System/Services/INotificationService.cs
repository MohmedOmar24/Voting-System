namespace Voting_System.Services;

public interface INotificationService
{
    Task SendNewVoteNotification(int? voteId=null);
}
