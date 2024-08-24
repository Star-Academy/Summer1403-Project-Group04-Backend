using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

public interface IUserRolesRemover
{
    Task RemoveUserRoles(User user);
}