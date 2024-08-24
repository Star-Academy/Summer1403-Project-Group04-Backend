using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices;

public class UserDeleter(IServiceProvider serviceProvider) : IUserDeleter
{
    public async Task DeleteUserAsync(User user)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Remove(user);
        await context.SaveChangesAsync();
    }
}