using System.Security.Claims;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class UserPasswordManagerService(ApplicationDbContext context, IUserReceiver userReceiver, IPasswordVerifier passwordVerifier, IPasswordHasher passwordHasher) : IUserPasswordManagerService
{
    public async Task<ActionResponce<MessageDto>> UpdatePasswordAsync(ClaimsPrincipal userClaims, UserPasswordInfoDto passwordInfoDto)
    {
        var result = new ActionResponce<MessageDto>();
        var user = await userReceiver.ReceiveUserAsync(userClaims);
        if (user is null)
        {
            return 
        }
        if (!passwordVerifier.VerifyPasswordHash(passwordInfoDto.OldPassword, user.PasswordHash))
        {
            result.Data = new MessageDto(Resources.WrongOldPasswordMessage);
            result.StatusCode = StatusCodeType.BadRequest;
            return result;
        }
        user.PasswordHash = passwordHasher.HashPassword(passwordInfoDto.NewPassword);
        context.Update(user);
        await context.SaveChangesAsync();
        
        result.Data = new MessageDto(Resources.SuccessfulUpdateUserMessage);
        return result;
    }
    
    
    private ActionResponce<MessageDto> NotFoundResult()
    {
        return new ActionResponce<MessageDto>()
        {
            Data = new MessageDto(Resources.UserNotFoundMessage),
            StatusCode = StatusCodeType.NotFound
        };
    }
}