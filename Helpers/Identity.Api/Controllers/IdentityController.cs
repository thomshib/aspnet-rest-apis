using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;


[ApiController]
public class IdentityController : ControllerBase
{

    private const string TokenSecret = "MaryHadALitteLambCalledDaisy";
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromHours(8);

    [HttpPost("token")]
    public IActionResult GenerateToken([FromBody] TokenGenerationRequest request)
    {

        var tokenHander = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(TokenSecret);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub,request.Email),
            new(JwtRegisteredClaimNames.Email,request.Email),
            new("userid",request.UserId.ToString())
        };

        foreach (var claimPair in request.CustomClaims)
        {
            var jsonElement = (JsonElement)claimPair.Value;
            var valueType = jsonElement.ValueKind switch
            {
                JsonValueKind.True => ClaimValueTypes.Boolean,
                JsonValueKind.False => ClaimValueTypes.Boolean,
                JsonValueKind.Number => ClaimValueTypes.Double,
                _ => ClaimValueTypes.String

            };

            var claim = new Claim(claimPair.Key, claimPair.Value.ToString()!, valueType);
            claims.Add(claim);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(TokenLifetime),
            Issuer = "https://id.localhost.com",
            Audience = "https://movies.localhost.com",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHander.CreateToken(tokenDescriptor) ;

        var jwt = tokenHander.WriteToken(token);
        
        return Ok(jwt);


    }



}