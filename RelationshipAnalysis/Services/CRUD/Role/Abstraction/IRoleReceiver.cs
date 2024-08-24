using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

public interface IRoleReceiver
{
    Task<List<string>> ReceiveRoleNamesAsync(int userId);

    Task<List<string>> ReceiveAllRolesAsync();

    Task<List<Role>> ReceiveRolesListAsync(List<string> roleNames);
}