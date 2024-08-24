using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices;

public class UserRolesRemover(IServiceProvider serviceProvider) : IUserRolesRemover
{
    public async Task RemoveUserRoles(User user)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var allUserRoles = await context.UserRoles.ToListAsync();
        var userRoles = allUserRoles.FindAll(r => r.UserId == user.Id);
        context.RemoveRange(userRoles);
        await context.SaveChangesAsync();
    }
}