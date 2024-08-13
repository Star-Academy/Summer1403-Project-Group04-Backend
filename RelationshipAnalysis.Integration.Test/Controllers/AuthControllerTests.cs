using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Controllers;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Integration.Test.Controllers;

public class HomeControllerTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();


    [Fact]
    public async Task Login_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var loginModel = new LoginDto
        {
            Username = "admin",
            Password = "admin"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<MessageDto>();
        Assert.NotNull(responseData);
        Assert.Equal("Login was successful!", responseData.Message);
    }
    
    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginModel = new LoginDto
        {
            Username = "testuser",
            Password = "invalidpassword" // Ensure this password does not match any hashed password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenNoCredentialsAreProvided()
    {
        // Act
        var loginModel = new LoginDto();
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginModel);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
    }
}