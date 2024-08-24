using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices;

public class UserUpdateRolesService(
    IRoleReceiver roleReceiver,
    IUserRolesAdder userRolesAdder,
    IMessageResponseCreator messageResponseCreator,
    IUserRolesRemover userRolesRemover,
    IServiceProvider serviceProvider) : IUserUpdateRolesService
{
    public async Task<ActionResponse<MessageDto>> UpdateUserRolesAsync(User user, List<string> newRoles)
    {
        if (newRoles.IsNullOrEmpty()) return messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.EmptyRolesMessage);

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var invalidRoles = newRoles.FindAll(r => !context.Roles.Select(role => role.Name)
            .Contains(r));
        if (invalidRoles.Any()) return messageResponseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidRolesListMessage);

        await userRolesRemover.RemoveUserRoles(user);
        var roles = await roleReceiver.ReceiveRolesListAsync(newRoles);
        await userRolesAdder.AddUserRoles(roles, user);
        
        return messageResponseCreator.Create(StatusCodeType.Success, Resources.SuccessfulUpdateRolesMessage);
    }
}