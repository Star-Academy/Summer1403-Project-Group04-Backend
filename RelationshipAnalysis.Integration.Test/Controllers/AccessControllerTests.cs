using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Newtonsoft.Json;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Models;

namespace RelationshipAnalysis.Integration.Test.Controllers;

public partial class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the app's ApplicationDbContext registration.
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add ApplicationDbContext using an in-memory database for testing.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Build the service provider.
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database
            // context (ApplicationDbContext).
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                // Ensure the database is created.
                db.Database.EnsureCreated();

                // Seed the database with test data.
                SeedDatabase(db);
            }
        });

        return base.CreateHost(builder);
    }

    private void SeedDatabase(ApplicationDbContext context)
    {
        // Seed the database with roles and permissions
        var userRole = new Role
        {
            Name = "User",
            Permissions = JsonConvert.SerializeObject(new List<string> { "Read" })
        };

        var adminRole = new Role
        {
            Name = "Admin",
            Permissions = JsonConvert.SerializeObject(new List<string> { "Read", "Write" })
        };

        context.Roles.AddRange(userRole, adminRole);

        var user = new User
        {
            Username = "testuser",
            PasswordHash = "hashedpassword",
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@example.com",
            UserRoles = new List<UserRole>
            {
                new UserRole { Role = userRole }
            }
        };

        context.Users.Add(user);
        context.SaveChanges();
    }
}

public static class JwtTokenHelper
{
    public static string GenerateToken()
    {
        var key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI"; // Ensure this key matches your JWT settings
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "testuser"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "testuser"), // Ensure this matches your application's claim type
            new Claim(ClaimTypes.Role, "User") // Ensure this role exists in your application
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}


public class AccessControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AccessControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnPermissions_WhenUserIsAuthorized()
    {
        // Arrange
        var token = JwtTokenHelper.GenerateToken(); // Generate a JWT token
        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/access/getpermissions");
        
        // Assert
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var permissionDto = JsonConvert.DeserializeObject<PermissionDto>(content);

            Assert.NotNull(permissionDto);
            Assert.Contains("Read", permissionDto.Permissions); // Adjust based on expected permissions
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.True(false, $"Request failed with status code: {response.StatusCode}. Error message: {errorMessage}");
        }
    }
}