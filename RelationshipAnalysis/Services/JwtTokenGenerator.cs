using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services.Abstractions;
using RelationshipAnalysis.Settings.JWT;

namespace RelationshipAnalysis.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _jwtSettings;
    private readonly SigningCredentials _creds;

    public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        _creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username)
        };
        
        user.UserRoles.ToList().ForEach(ur =>
            claims.Add(new Claim(ClaimTypes.Role, ur.Role.Name)));

        var token = TokenGenerator(claims);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private JwtSecurityToken TokenGenerator(List<Claim> claims)
    {
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: _creds);
        return token;
    }
}