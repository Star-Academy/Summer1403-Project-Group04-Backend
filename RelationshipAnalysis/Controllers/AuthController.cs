using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Services.Abstractions;
using RelationshipAnalysis.Settings.JWT;

namespace RelationshipAnalysis.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ICookieSetter _cookieSetter;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordVerifier _passwordVerifier;

    public AuthController(ApplicationDbContext context, ICookieSetter cookieSetter,
        IJwtTokenGenerator jwtTokenGenerator, IPasswordVerifier passwordVerifier)
    {
        _context = context;
        _cookieSetter = cookieSetter;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordVerifier = passwordVerifier;
    }
    
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginModel)
    {
        var user = await _context.Users
            .SingleOrDefaultAsync(u => u.Username == loginModel.Username);
        
        if (user == null || !_passwordVerifier.VerifyPasswordHash(loginModel.Password, user.PasswordHash))
        {
            return Unauthorized();
        }

        var token = _jwtTokenGenerator.GenerateJwtToken(user);
        _cookieSetter.SetCookie(Response, token);

        return Ok("Login was successful!");
    }
}