using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Auth;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.AuthServices.Abstraction;

namespace RelationshipAnalysis.Services.AuthServices;

public class LoginService(
    IServiceProvider serviceProvider,
    ICookieSetter cookieSetter,
    IJwtTokenGenerator jwtTokenGenerator,
    IPasswordVerifier passwordVerifier)
    : ILoginService
{
    public async Task<ActionResponse<MessageDto>> LoginAsync(LoginDto loginModel, HttpResponse response)
    {
        var result = new ActionResponse<MessageDto>();
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var user = await context.Users
            .SingleOrDefaultAsync(u => u.Username == loginModel.Username);

        if (user == null || !passwordVerifier.VerifyPasswordHash(loginModel.Password, user.PasswordHash))
        {
            result.Data = new MessageDto(Resources.LoginFailedMessage);
            result.StatusCode = StatusCodeType.Unauthorized;
            return result;
        }

        var token = jwtTokenGenerator.GenerateJwtToken(user);
        cookieSetter.SetCookie(response, token);

        result.Data = new MessageDto(Resources.SuccessfulLoginMessage);
        return result;
    }
}