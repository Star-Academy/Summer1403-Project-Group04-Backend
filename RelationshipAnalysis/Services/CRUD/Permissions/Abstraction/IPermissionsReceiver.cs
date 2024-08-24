using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.CRUD.Permissions;

public interface IPermissionsReceiver
{
    Task<List<string>> ReceivePermissionsAsync(Models.Auth.User user);
}