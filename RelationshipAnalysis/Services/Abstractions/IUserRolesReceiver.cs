using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Services.Abstractions;

public interface IUserRolesReceiver
{
    List<string> ReceiveRoles(int userId);
}