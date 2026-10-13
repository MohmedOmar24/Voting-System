
using Mapster;
using Voting_System.Authentication.Filters;
using Microsoft.AspNetCore.Authorization;
using Voting_System.Abstractions.Consts;
using Voting_System.Contracts.Votes;
//using Voting_System.Mapping;

namespace Voting_System.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class VotesController(IVoteService voteService) : ControllerBase
{
    private readonly IVoteService _voteService = voteService;
    // private readonly IMapper _mapper = mapper;
   
    
    [HttpGet("")]
    [HasPermission(Permissions.GetVotes)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        
            var votes = await _voteService.GetAllAsync(cancellationToken);
            return Ok(votes);
        
       
    }


    [HttpGet("current")]
    [Authorize(Roles = DefaultRoles.Member)]

    public async Task<IActionResult> GetCurrent(CancellationToken cancellationToken = default)
    {
        
            var votes = await _voteService.GetCurrentAsync(cancellationToken);
            return Ok(votes);
        

    }


    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken = default)
    {
        var result= await _voteService.GetAsync(id, cancellationToken);
        return result.IsSuccess ?
     Ok(result.Value) :
     Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);
             // NotFound(result.Error);
    }




    [HttpPost("")]
    public async Task<IActionResult> Add(VoteRequest request, CancellationToken cancellationToken)
    {

        var result = await _voteService.AddAsync(request, cancellationToken);
        return result.IsSuccess ?
                     CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                     : Problem(statusCode: StatusCodes.Status500InternalServerError, title: result.Error.Code, detail: result.Error.Description);
        //return CreatedAtAction(nameof(Add), new { id = vote.Id }, vote.Adapt<VoteResponse>());

    }



    [HttpPut("{id}")]
    public async Task<IActionResult>Update([FromRoute] int id, VoteRequest request, CancellationToken cancellationToken )
    {
        var result = await _voteService.UpdateAsync(id, request,cancellationToken);
        
        return result.IsSuccess? NoContent()
                    : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);

        //NotFound(result.Error);
    }





    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken )
    { 
        var deltedResult= await _voteService.DeleteAsync(id,cancellationToken);

        return deltedResult.IsSuccess? NoContent()
             : Problem(statusCode: StatusCodes.Status404NotFound, title: deltedResult.Error.Code, detail: deltedResult.Error.Description);

        //NotFound(deltedResult.Error);

    }

    [HttpPut("{id}/togglepublish")]
    public async Task<IActionResult> TogglePublish(int id, CancellationToken cancellationToken)
    {
        var result = await _voteService.TogglePublishAsync(id, cancellationToken);

        return result.IsSuccess ? NoContent()
             : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);


    }

}
