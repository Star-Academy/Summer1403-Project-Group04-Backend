using Newtonsoft.Json;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.CRUD.Permissions;

public class PermissionsReceiver(
    IRoleReceiver roleReceiver) : IPermissionsReceiver
{
    public async Task<List<string>> ReceivePermissionsAsync(Models.Auth.User user)
    {
        var roleNames = await roleReceiver.ReceiveRoleNamesAsync(user.Id);
        var roles = await roleReceiver.ReceiveRolesListAsync(roleNames);
        var unionList = new HashSet<string>();

        foreach (var role in roles)
        {
            var newList = JsonConvert.DeserializeObject<List<string>>(role.Permissions) ?? [];
            unionList.UnionWith(newList);
        }

        return unionList.ToList();
    }
}