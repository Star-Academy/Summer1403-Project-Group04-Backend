using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Services.Panel.AdminPanelServices.Abstraction;

namespace RelationshipAnalysis.Services.Panel.AdminPanelServices;

public class UserRolesAdder(IServiceProvider serviceProvider) : IUserRolesAdder
{
    public async Task AddUserRoles(List<Role> roles, User user)
    {
        var userRoles = roles.Select(role => new UserRole
        {
            RoleId = role.Id,
            UserId = user.Id
        }).ToList();

        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.UserRoles.AddRangeAsync(userRoles);
        await context.SaveChangesAsync();
    }
}