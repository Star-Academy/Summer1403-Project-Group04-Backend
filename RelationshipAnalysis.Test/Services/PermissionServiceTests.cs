using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services;

namespace RelationshipAnalysis.Test.Services;

public class PermissionServiceTests
{
    private readonly PermissionService _permissionService;
    private readonly ApplicationDbContext _context;

    public PermissionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);

        // Seed the database with test data
        SeedDatabase();

        _permissionService = new PermissionService(_context);
    }

    private void SeedDatabase()
    {
        // Seed the database with sample roles
        _context.Roles.AddRange(new List<Role>
        {
            new Role
            {
                Name = "User",
                Permissions = JsonConvert.SerializeObject(new List<string> { "Read", "Write" })
            },
            new Role
            {
                Name = "Admin",
                Permissions = JsonConvert.SerializeObject(new List<string> { "Delete", "Write" })
            }
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnPermissions_WhenRolesExist()
    {
        // Arrange
        var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Role, "Admin")
        }));

        // Act
        var result = await _permissionService.GetPermissionsAsync(userClaims);

        // Assert
        var expectedPermissions = JsonConvert.SerializeObject(new List<string> { "Read", "Write", "Delete" });
        Assert.Equal(expectedPermissions, result.Data.Permissions);
    }

    [Fact]
    public async Task GetPermissionsAsync_ShouldReturnDefaultPermissions_WhenNoRolesExist()
    {
        // Arrange
        var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Role, "NonExistentRole")
        }));

        // Act
        var result = await _permissionService.GetPermissionsAsync(userClaims);

        // Assert
        var expectedPermissions = JsonConvert.SerializeObject(new List<string> { "Read", "Write" });
        Assert.Equal(expectedPermissions, result.Data.Permissions);
    }
}