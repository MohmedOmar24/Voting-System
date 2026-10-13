namespace Voting_System.Entities;

public sealed class VoteSubmission
{
    public int Id { get; set; }

    public int VoteId { get; set; }
    public Vote Vote { get; set; } = default!;


    public string UserId { get; set; }=string.Empty;
    public ApplicatonUser User { get; set; }=default!;

    public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;


    public ICollection<VoteSubmissionAnswer> VoteSubmissionAnswers { get; set; } = [];

    
}
