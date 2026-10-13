using Microsoft.AspNetCore.Authorization;
using Voting_System.Errors;
using Voting_System.Contracts.Questions;

namespace Voting_System.Controllers;
[Route("api/votes/{voteId}/[controller]")]
[ApiController]
//[Authorize]
public class QuestionsController(IQuestionService questionService) : ControllerBase
{

    private readonly IQuestionService _questionService=questionService;


    [HttpGet("")]
    public async Task<IActionResult> GetAll([FromRoute]int voteId,CancellationToken cancellationToken)
    {
        var result= await _questionService.GetAllAsync(voteId, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
    
    
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get([FromRoute]int voteId, [FromRoute]int id,CancellationToken cancellationToken)
    {
        var result=await _questionService.GetAsync(voteId, id, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);

    }




    [HttpPost("")] 
    public async Task<IActionResult> Add([FromRoute] int voteId,QuestionRequest request,CancellationToken cancellationToken)
    {
        var result= await _questionService.AddAsync(voteId,request,cancellationToken);
        return result.IsSuccess ? CreatedAtAction(nameof(Get), new {voteId,result.Value.Id},result.Value)
            
            : BadRequest(result.Error);
    }


    [HttpPut("id")]
    public async Task<IActionResult> Update([FromRoute] int voteId, [FromRoute] int id, QuestionRequest request, CancellationToken cancellationToken)
    {
       
        var result = await _questionService.UpdateAsync(voteId,id, request, cancellationToken);
        if (result.IsSuccess)
            return NoContent();

        return result.Error.Equals(QuestionErrors.QuestionNotFound) ?
             Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description)
           : Problem(statusCode: StatusCodes.Status409Conflict, title: result.Error.Code, detail: result.Error.Description);


    }

    [HttpPut("{id}/toggleStatus")]
    public async Task<IActionResult> ToggleStatus([FromRoute] int voteId, [FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _questionService.ToggleStatusAsync(voteId,id, cancellationToken);

        return result.IsSuccess ? NoContent()
             : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.Code, detail: result.Error.Description);


    }


}
