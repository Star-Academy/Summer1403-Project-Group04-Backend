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
    public async Task<ActionResponse<MessageDto>> UpdatePasswordAsync(ClaimsPrincipal userClaims, UserPasswordInfoDto passwordInfoDto)
    {
        var result = new ActionResponse<MessageDto>();
        var user = await userReceiver.ReceiveUserAsync(userClaims);
        if (user is null)
        {
            return NotFoundResult();
        }
        if (!passwordVerifier.VerifyPasswordHash(passwordInfoDto.OldPassword, user.PasswordHash))
        {
            return WrongPasswordResult();
        }
        user.PasswordHash = passwordHasher.HashPassword(passwordInfoDto.NewPassword);
        context.Update(user);
        await context.SaveChangesAsync();

        return SuccessResult();
    }
    
    
    private ActionResponse<MessageDto> NotFoundResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.UserNotFoundMessage),
            StatusCode = StatusCodeType.NotFound
        };
    }

    private ActionResponse<MessageDto> WrongPasswordResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.WrongOldPasswordMessage),
            StatusCode = StatusCodeType.BadRequest
        };
    }
    
    private ActionResponse<MessageDto> SuccessResult()
    {
        return new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulUpdateUserMessage),
            StatusCode = StatusCodeType.Success
        };
    }


}