using RelationshipAnalysis.Context;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Services;

public class UserRolesReceiver(ApplicationDbContext context) : IUserRolesReceiver
{
    public List<string> ReceiveRoles(int userId)
    {
        return context.UserRoles.ToList().FindAll(ur => ur.UserId == userId)
            .Select(ur => ur.Role.Name).ToList();
    }
}