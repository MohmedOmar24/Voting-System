using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Voting_System.Extensions;

using Voting_System.Errors;
using Voting_System.Abstractions.Consts;
using Voting_System.Contracts.VoteSubmissions;


namespace Voting_System.Controllers;
[Route("api/votes/{voteId}/[controller]")]
[ApiController]
[Authorize(Roles=DefaultRoles.Member)]
public class VoteSubmissionsController(IQuestionService questionService,
                                       IVoteSubmissionService voteSubmissionService) : ControllerBase
{
    private readonly IQuestionService _questionService=questionService;
    private readonly IVoteSubmissionService _voteSubmissionService=voteSubmissionService;

    [HttpGet("")]
    public async Task<IActionResult> Start([FromRoute]int voteId,CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        //User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await _questionService.GetAvalaibleAsync(voteId, userId, cancellationToken);

        if (result.IsSuccess)
            return Ok(result.Value);
        
        return result.Error.Equals(VoteErrors.VoteNotFound) ?
            Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description)
          : Problem(statusCode: StatusCodes.Status409Conflict, title: result.Error.Code, detail: result.Error.Description);

    }
    [HttpPost("")]
    public async Task<IActionResult> VoteSubmit ([FromRoute]int voteId, [FromBody] VoteSubmissionRequest request, CancellationToken cancellationToken)

    {

        var result = await _voteSubmissionService.AddAsync(voteId, User.GetUserId()!, request, cancellationToken);
        if (result.IsSuccess)
            return Created();
         return Problem(statusCode: StatusCodes.Status409Conflict, title: result.Error.Code, detail: result.Error.Description);

    }
}
