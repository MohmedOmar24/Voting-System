namespace Voting_System.Entities;

public sealed class VoteSubmissionAnswer
{

    public int Id { get; set; }

  
    public int VoteSubmissionId { get; set; }
    public VoteSubmission VoteSubmission { get; set; } = default!;

  
    public int QuestionId { get; set; }
    public Question Question { get; set; }=default!;

   
    public int AnswerId { get; set; }
    public Answer Answer { get; set; }=default!;


}
