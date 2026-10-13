using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Voting_System.Contracts.Authentication;

namespace Voting_System.Controllers;
[Route("[controller]")]
[ApiController]
public class AuthController(IAuthService authService,IConfiguration configuration) : ControllerBase
{
    private readonly IAuthService _authService=authService;
    private readonly IConfiguration _configuration=configuration;
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest,CancellationToken cancellationToken)
    {
        var authresult= await _authService.GetTokenAsync(loginRequest.Email, loginRequest.Password,cancellationToken);

        return authresult.IsSuccess ? Ok(authresult.Value)
            : Problem(statusCode: StatusCodes.Status400BadRequest, title: authresult.Error.Code, detail: authresult.Error.Description);
    }



    [HttpPost("refresh")]
    public async Task<IActionResult> refresh([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
    {
        var authresult = await _authService.GetRefreshTokenAsync(refreshTokenRequest.Token,refreshTokenRequest.RefreshToken, cancellationToken);

        return authresult.IsFailure ?
            Problem(statusCode: StatusCodes.Status400BadRequest, title: authresult.Error.Code, detail: authresult.Error.Description) 
            : Ok(authresult.Value);
    }

    [HttpPost("revok-refresh-token")]
    public async Task<IActionResult> revokerefreshtoken([FromBody]RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest("Invalid opertion");
    }




    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest registerRequest,CancellationToken cancellationToken)
    {
        var result= await _authService.RegisterAsync(registerRequest,cancellationToken);

        return result.IsSuccess? Ok() : BadRequest(result.Error);
           
    }
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
    {
        var result = await _authService.ConfirmEmailAsync(request);
       return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailRequest request)
    {
        var result = await _authService.ResendConfirmationEmailAsync(request);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest request)
    {
        var result = await _authService.SendResetPasswordCodeAsync(request.Email);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);
        return result.IsSuccess ? Ok() : Unauthorized(result.Error);
    }
}
