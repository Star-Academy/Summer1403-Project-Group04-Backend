using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;
using Microsoft.Extensions.Configuration;

namespace RelationAnalysis.Migrations;

public class InitialRecordsCreator
{
    private IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json")
        .Build();

    public void AddInitialRecords(ApplicationDbContext context)
    {
        var roles = new List<Role>()
        {
            new Role() { Name = "Admin", Permissions = "[]", Id = 1 },
            new Role() { Name = "DataAdmin", Permissions = "[]", Id = 2 },
            new Role() { Name = "DataAnalyst", Permissions = "[]", Id = 3 }
        };
        var userRoles = new List<UserRole>()
        {
            new UserRole()
            {
                UserId = 1,
                RoleId = 1
            },
            new UserRole()
            {
                UserId = 1,
                RoleId = 2
            },
            new UserRole()
            {
                UserId = 1,
                RoleId = 3
            }
        };
        var users = new List<User>()
        {
            new User()
            {
                Username = "admin",
                PasswordHash = Configuration["admin"],
                FirstName = "FirstName",
                LastName = "LastName",
                Email = "admin@gmail.com",
                Id = 1,
            }
        };

        context.AddRangeAsync(roles);
        context.AddRangeAsync(users);
        context.AddRangeAsync(userRoles);
        context.SaveChangesAsync();
    }
}