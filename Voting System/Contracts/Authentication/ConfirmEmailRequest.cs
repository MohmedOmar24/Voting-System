namespace Voting_System.Contracts.Authentication;

public class ConfirmEmailRequest
{
    public string UserId { get; set; }
    public string Code {  get; set; }
}
