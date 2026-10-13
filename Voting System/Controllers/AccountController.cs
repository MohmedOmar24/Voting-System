using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Voting_System.Contracts.Users;
using Voting_System.Extensions;

namespace Voting_System.Controllers;
//[Route("me")]
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AccountController(
                          IAccountService accountService) : ControllerBase
{
    private readonly IAccountService _accountService=accountService;



    [HttpGet("")]
    public async Task<IActionResult> Info()
    {
        var result =await _accountService.GetProfileAsync(User.GetUserId()!);
        return Ok(result.Value);

    }
    [HttpPut("info")]
    public async Task<IActionResult> Info(UpdateProfileRequest request)
    {
         await _accountService.UpdateProfileAsync(User.GetUserId()!,request);
        return NoContent();

    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
       var result= await _accountService.ChangePasswordAsync(User.GetUserId()!, request);
        if (result.IsSuccess)
        return NoContent();

        return BadRequest(result.Error);

    }
}
