using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

public interface IUserDeleter
{
    Task DeleteUserAsync(User user);
}