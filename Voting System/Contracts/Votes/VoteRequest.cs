namespace Voting_System.Contracts.Votes;

public class VoteRequest
{
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateOnly StartsAt { get; set; }
    public DateOnly EndsAt { get; set; }

    /*public static explicit operator Vote(CreateVoteRequest request)
    {
        return
        new()
          {
            
            Title = request.Title,
              Description = request.Description
          };
    }*/
}

