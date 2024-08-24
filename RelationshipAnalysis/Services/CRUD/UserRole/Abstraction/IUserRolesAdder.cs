using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

public interface IUserRolesAdder
{
    Task AddUserRoles(List<Role> roles, User user);
}