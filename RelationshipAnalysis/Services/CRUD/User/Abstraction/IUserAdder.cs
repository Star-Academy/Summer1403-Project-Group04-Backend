using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

public interface IUserAdder
{
    Task<User> AddUserAsync(User user);
}