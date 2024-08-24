using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.UserPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.UserPanelServices;

public class UserAdder(IServiceProvider serviceProvider) : IUserAdder
{
    public async Task<User> AddUserAsync(User user)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }
}