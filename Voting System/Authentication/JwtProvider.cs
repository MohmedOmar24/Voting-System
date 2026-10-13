
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Voting_System.Authentication;

public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _options=options.Value;
    public (string token, int expiresIn) GenerateToken(ApplicatonUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        Claim[] claims = [
        new(JwtRegisteredClaimNames.Sub,user.Id),
        new(JwtRegisteredClaimNames.Email,user.Email!),
        new(JwtRegisteredClaimNames.GivenName,user.FirstName),
        new(JwtRegisteredClaimNames.FamilyName,user.LastName),
        new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        new(nameof(roles),JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
        new(nameof(permissions),JsonSerializer.Serialize(permissions),JsonClaimValueTypes.JsonArray)
            ];
        var symmetricsecurtykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var signedCredentials= new SigningCredentials(symmetricsecurtykey,SecurityAlgorithms.HmacSha256);

        var expiredin = _options.expiryin;

        var token = new JwtSecurityToken(
            issuer: _options.issuer,
            audience:_options.audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiredin),
            signingCredentials: signedCredentials
            );

        return(token:new JwtSecurityTokenHandler().WriteToken(token),expiresIn:expiredin*60);
    }
    public string? ValidateToken (string token)
    {
        var TokenHandler = new JwtSecurityTokenHandler();
        var symmetricsecurtykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        try
        {
            TokenHandler.ValidateToken(token, new TokenValidationParameters()
            {
                IssuerSigningKey = symmetricsecurtykey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew=TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var JwtToken =(JwtSecurityToken) validatedToken;
          
            return JwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;
        }

    }
}
