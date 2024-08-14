using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Newtonsoft.Json;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Settings.JWT;

namespace RelationshipAnalysis.Integration.Test.Controllers;

 public class CustomWebApplicationFactory2<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing ApplicationDbContext registration
            var descriptor =
                services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add ApplicationDbContext using an in-memory database for testing
            services.AddDbContext<ApplicationDbContext>(options => { options.UseInMemoryDatabase("TestDatabase"); });

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to get a reference to the database context
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<ApplicationDbContext>();

                // Ensure the database is created and seed it
                dbContext.Database.EnsureCreated();
                SeedDatabase(dbContext);
            }
        });
    }

    private void SeedDatabase(ApplicationDbContext dbContext)
    {
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com"
        };
        var role = new Role()
        {
            Id = 1,
            Name = "admin",
            Permissions = "[\"AdminPermissions\"]",
        };
        var userRole = new UserRole()
        {
            Id = 1,
            Role = role,
            User = user
        };
        user.UserRoles.Add(userRole);
        role.UserRoles.Add(userRole);
        dbContext.Users.Add(user);
        dbContext.Roles.Add(role);
        dbContext.UserRoles.Add(userRole);
        dbContext.SaveChanges();
    }
}
public class AccessControllerTests : IClassFixture<CustomWebApplicationFactory2<Program>>
{
    private readonly HttpClient _client;

    public AccessControllerTests(CustomWebApplicationFactory2<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnPermissions_WhenUserIsAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/access/getpermissions");
        var jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(jwtSettings);
        
        

        var user = new User()
        {
            Username = "Test",
            UserRoles = new List<UserRole>(){new UserRole(){Role = new Role(){Name = "admin"}}}
        };
        
        // Set the JWT token for authenticated requests
        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);// Generate or mock a valid token
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<PermissionDto>();
        Assert.NotNull(responseData);
        Assert.NotEmpty(responseData.Permissions); // Assuming PermissionsDto has a Permissions list
    }

    [Fact]
    public async Task GetPermissions_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
    {
        // Remove the JWT token for unauthorized requests
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync("/api/access/getpermissions");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
}