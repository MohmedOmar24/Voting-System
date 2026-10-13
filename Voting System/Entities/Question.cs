namespace Voting_System.Entities;

public sealed class Question:AuditableEntity
{
    public int Id { get; set; }
    public string Content {  get; set; }=string.Empty;
    public bool IsActive { get; set; } = true;
    
    
    public int VoteId { get; set; }
    public Vote Vote { get; set; } = default!;


    public ICollection<Answer> Answers { get; set; } = [];

}
