using RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices;

public class LogoutService : ILogoutService
{
    public void Logout(HttpResponse response)
    {
        response.Cookies.Delete("jwt");
    }
}