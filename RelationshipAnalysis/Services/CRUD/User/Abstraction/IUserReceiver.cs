using System.Security.Claims;
using RelationshipAnalysis.Models.Auth;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

public interface IUserReceiver
{
    Task<int> ReceiveAllUserCountAsync();
    Task<User> ReceiveUserAsync(ClaimsPrincipal userClaims);
    Task<User> ReceiveUserAsync(int id);
    Task<User> ReceiveUserAsync(string username);
    List<User> ReceiveAllUserAsync(int page, int size);
}