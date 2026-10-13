using System.Security.Cryptography;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Voting_System.Authentication;
using Voting_System.Abstractions.Consts;
using Voting_System.Persistence;
using Voting_System.Contracts.Authentication;
using Voting_System.Errors;
using Voting_System.Helpers;
using System.Threading;
using Hangfire;

namespace Voting_System.Services;

public class AuthService(UserManager<ApplicatonUser> userManager,
                         SignInManager<ApplicatonUser> signInManager,
                         ApplicationDbContext DbContext,
                         ILogger<AuthService> logger,
                          IJwtProvider jwtProvider,
                          IEmailSender emailsender,
                          IHttpContextAccessor contextAccessor) :IAuthService
{
    private readonly UserManager<ApplicatonUser> _userManager = userManager;
    private readonly SignInManager<ApplicatonUser> _signInManager = signInManager;
    private readonly ApplicationDbContext _context=DbContext;
    private readonly IJwtProvider _jwtProvider=jwtProvider;
    private readonly ILogger<AuthService> _logger=logger;
    private readonly IEmailSender _emailSender=emailsender;
    private readonly IHttpContextAccessor _contextAccessor=contextAccessor;
    private readonly int _RefreshTokenExpiryDays = 14;




    public async Task<Result<AuthResponse>> GetTokenAsync(string Email, string Password, CancellationToken cancellationToken = default)
    {
        
        if (await _userManager.FindByEmailAsync(Email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentilas);

        if(user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);


        //var isvalidPass=   await _userManager.CheckPasswordAsync(user, Password);
        //if (!isvalidPass)
        //    return null;
        var result = await _signInManager.PasswordSignInAsync(user, Password, false, true);
        
        if(result.Succeeded)
        {

            var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);
            var (token, expiresin) = _jwtProvider.GenerateToken(user,userRoles, userPermissions);
            var RefreshToken = GenerateRefreshToken();
            var RefreshTokenExpiration = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken()
            {
                Token = RefreshToken,
                ExpiresOn = RefreshTokenExpiration
            });
           await  _userManager.UpdateAsync(user);
            var response= new AuthResponse()
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = token,
                ExpiredIn = expiresin,
                RefreshToken=RefreshToken,
                RefreshTokenExpiratin= RefreshTokenExpiration
            };

            return Result.Success(response);

        }

        var error = result.IsNotAllowed ?
                   UserErrors.EmailNotConfirmed :
                   result.IsLockedOut ?
                   UserErrors.LockedUser :
                   UserErrors.InvalidCredentilas;
       
        return Result.Failure<AuthResponse>(error);
        
    }
    



    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token,string refreshtoken,CancellationToken cancellationToken=default)
    {
        var userId=_jwtProvider.ValidateToken(token);
       
        if(userId == null) 
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);
           

        var user=await _userManager.FindByIdAsync(userId);
        
        if(user == null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        if (user.LockoutEnd>DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshtoken && x.IsActive);
        
        if(userRefreshToken == null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);


        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (userRoles, userPermissions) = await GetUserRolesAndPermissions(user, cancellationToken);
        
        var (newtoken, expiresin) = _jwtProvider.GenerateToken(user, userRoles, userPermissions!);
        var newRefreshToken = GenerateRefreshToken();
        var RefreshTokenExpiration = DateTime.UtcNow.AddDays(_RefreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken()
        {
            Token = newRefreshToken,
            ExpiresOn = RefreshTokenExpiration
        });
        await _userManager.UpdateAsync(user);
        var response= new AuthResponse()
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = newtoken,
            ExpiredIn = expiresin,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiratin = RefreshTokenExpiration
        };

        return Result.Success(response);



    }


    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshtoken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);
       
        if (userId == null) 
            return Result.Failure(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Result.Failure(UserErrors.InvalidJwtToken);
        

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshtoken && x.IsActive);
       
        if (userRefreshToken == null)
            return Result.Failure(UserErrors.InvalidRefreshToken);


        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);
        return Result.Success();
    }


    public async Task<Result> RegisterAsync(RegisterRequest request,CancellationToken cancellationToken= default)
    {
        var emailIsExist= await _userManager.Users.AnyAsync(x=>x.Email == request.Email,cancellationToken);

        if (emailIsExist)
            return Result.Failure(UserErrors.DublicatedEmail);

        var user = request.Adapt<ApplicatonUser>();
        user.UserName = request.Email;

        var result = await _userManager.CreateAsync(user, request.Password);

        if(result.Succeeded)
        {
            
            var code= await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            _logger.LogInformation("Confirmation code {code}", code);


            await SendConfirmationEmail (user, code);
           
            //send email
            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DublicatedConfirmation);


        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {

            await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
            return Result.Success();

        }

        var error = result.Errors.First();
           return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }


    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DublicatedConfirmation);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Confirmation code {code}", code);
        //send email
        await SendConfirmationEmail(user, code);

        return Result.Success();

    }


    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if(await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);


        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation("Reset code {code}", code);


        await SendResetPasswordEmail(user, code);

        //send email
        return Result.Success();


    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result= await _userManager.ResetPasswordAsync(user, code,request.NewPassword);

        }
        catch(FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());

        }

        if (result.Succeeded)
            return Result.Success();

        var error= result.Errors.First();
          return Result.Failure(new Error(error.Code,error.Description,StatusCodes.Status401Unauthorized));

    }


    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
    private async Task SendConfirmationEmail(ApplicatonUser user,string code)
    {
        var origin = _contextAccessor.HttpContext?.Request.Headers.Origin;
        var emailbody = EmailBodyBuilder.GenerateEmailBody("EmailConfiramtion",
            new Dictionary<string, string>
            {
                    {"{{name}}",user.FirstName },
                    {"{{action_url}}", $"{origin}/auth/emailConfirmation?userid={user.Id}&code={code}" }
            }
            );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Voting System : email confirmation", emailbody));
        await Task.CompletedTask;
    }
    private async Task SendResetPasswordEmail(ApplicatonUser user, string code)
    {
        var origin = _contextAccessor.HttpContext?.Request.Headers.Origin;
        var emailbody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
             new Dictionary<string, string>
            {
                    {"{{name}}",user.FirstName },
                    {"{{action_url}}", $"{origin}/auth/forgetPassword?email={user.Email}&code={code}" }
            }
            );

        BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Voting System : Change Password", emailbody));
        await Task.CompletedTask;
    }

    private async Task<(IEnumerable<string> userRoles,IEnumerable<string> userPermissions)> GetUserRolesAndPermissions(ApplicatonUser user,CancellationToken cancellationToken=default)
    {
      
        var userRoles = await _userManager.GetRolesAsync(user);
         
      /*  var userPermissions = await _context.Roles.Join(
                                   _context.RoleClaims,
                                   role => role.Id, claims => claims.RoleId,
                                  (role, claims) => new { role, claims })
                                 .Where(x => userRoles.Contains(x.role.Name!))
                                 .Select(x => x.claims.ClaimValue)
                                 .Distinct()
                                 .ToListAsync(cancellationToken);*/
      var userPermissions=await (from r in _context.Roles join p in _context.RoleClaims
                                 on r.Id equals p.RoleId
                                 where userRoles.Contains(r.Name!) 
                                 select p.ClaimValue)
                                 .Distinct()
                                 .ToListAsync(cancellationToken);
        return (userRoles, userPermissions!);
    }
}
