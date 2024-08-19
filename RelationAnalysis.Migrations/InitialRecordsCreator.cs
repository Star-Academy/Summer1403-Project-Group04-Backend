using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Auth;

namespace RelationAnalysis.Migrations;

public class InitialRecordsCreator
{
    public void AddInitialRecords(ApplicationDbContext context)
    {
        var roles = new List<Role>()
        {
            new Role() { Name = "Admin", Permissions = "[]" },
            new Role() { Name = "DataAdmin", Permissions = "[]" },
            new Role() { Name = "DataAnalyst", Permissions = "[]" }
        };
        context.AddRangeAsync(roles);
        context.AddAsync(new User()
        {
            Email = "admin@gmail.com",
            FirstName = "FirstName",
            LastName = "LastName",
            PasswordHash = 
            
        })
        context.SaveChanges();
    }
}