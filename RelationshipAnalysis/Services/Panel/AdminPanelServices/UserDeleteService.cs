using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices;

public class UserDeleteService(
    IMessageResponseCreator messageResponseCreator,
    IUserDeleter userDeleter)
    : IUserDeleteService
{
    public async Task<ActionResponse<MessageDto>> DeleteUser(User user)
    {
        if (user is null) return messageResponseCreator.Create(StatusCodeType.NotFound, Resources.UserNotFoundMessage);

        await userDeleter.DeleteUserAsync(user);

        return messageResponseCreator.Create(StatusCodeType.Success, Resources.SuccessfulDeleteUserMessage);
    }
}