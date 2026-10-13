
namespace Voting_System.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentilas = 
        new("User.InvalidCredentilas", "Invalid Email/Password", StatusCodes.Status401Unauthorized);

    public static readonly Error DisabledUser =
        new("User.DisabledUser", "this account is disabled", StatusCodes.Status401Unauthorized);

    public static readonly Error LockedUser =
       new("User.LockedUser", "locked user", StatusCodes.Status401Unauthorized);

    public static readonly Error EmailNotConfirmed =
       new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

    public static readonly Error DublicatedEmail =
       new("User.DublicatedEmail", "Email is exist", StatusCodes.Status409Conflict);

    public static readonly Error InvalidJwtToken =
      new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidCode =
       new("User.InvalidCode", "Invalid Code", StatusCodes.Status401Unauthorized);
  
    public static readonly Error DublicatedConfirmation =
      new("User.DublicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);
}
