using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Panel.User;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.AuthServices.Abstraction;
using RelationshipAnalysis.Services.CRUD.User.Abstraction;
using RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices;

public class UserPasswordService(
    IMessageResponseCreator messageResponseCreator,
    IUserUpdater userUpdater,
    IPasswordVerifier passwordVerifier,
    IPasswordHasher passwordHasher) : IUserPasswordService
{
    public async Task<ActionResponse<MessageDto>> UpdatePasswordAsync(User user, UserPasswordInfoDto passwordInfoDto)
    {
        if (user is null) return messageResponseCreator.Create(StatusCodeType.NotFound, Resources.UserNotFoundMessage);
        if (!passwordVerifier.VerifyPasswordHash(passwordInfoDto.OldPassword, user.PasswordHash))
            return messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.WrongOldPasswordMessage);
        
        user.PasswordHash = passwordHasher.HashPassword(passwordInfoDto.NewPassword);

        await userUpdater.UpdateUserAsync(user);
        
        return messageResponseCreator.Create(StatusCodeType.Success, Resources.SuccessfulUpdateUserMessage);
    }
}