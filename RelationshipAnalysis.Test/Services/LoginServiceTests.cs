using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.DTO;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Test.Services;

public class TestDatabaseFixture : IDisposable
{
    public ApplicationDbContext Context { get; private set; }

    public TestDatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        Context = new ApplicationDbContext(options);

        // Seed the database with initial data if necessary
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        // Add test data to the database
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = "correctPasswordHash", // This should be the hash of "correctPassword"
            FirstName = "Test",
            LastName = "User",
            Email = "testuser@example.com"
        };
        Context.Users.Add(user);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context?.Dispose();
    }
}

public class LoginServiceTests : IClassFixture<TestDatabaseFixture>
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<ICookieSetter> _mockCookieSetter;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly Mock<IPasswordVerifier> _mockPasswordVerifier;
    private readonly LoginService _loginService;
    private readonly Mock<HttpResponse> _mockHttpResponse;
    private readonly Mock<IResponseCookies> _mockResponseCookies;

    public LoginServiceTests(TestDatabaseFixture fixture)
    {
        _context = fixture.Context;
        _mockCookieSetter = new Mock<ICookieSetter>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _mockPasswordVerifier = new Mock<IPasswordVerifier>();
        
        _mockResponseCookies = new Mock<IResponseCookies>();
        _mockHttpResponse = new Mock<HttpResponse>();
        _mockHttpResponse.SetupGet(r => r.Cookies).Returns(_mockResponseCookies.Object);

        _loginService = new LoginService(
            _context,
            _mockCookieSetter.Object,
            _mockJwtTokenGenerator.Object,
            _mockPasswordVerifier.Object
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "correctPassword" };
        var token = "validToken";

        _mockPasswordVerifier.Setup(p => p.VerifyPasswordHash(loginDto.Password, It.IsAny<string>()))
            .Returns(true);
        _mockJwtTokenGenerator.Setup(j => j.GenerateJwtToken(It.IsAny<User>()))
            .Returns(token);

        // Act
        var result = await _loginService.LoginAsync(loginDto, _mockHttpResponse.Object);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Equal("Login was successful!", result.Data.Message);
        _mockCookieSetter.Verify(c => c.SetCookie(_mockHttpResponse.Object, token), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "nonexistentuser", Password = "password" };

        // Act
        var result = await _loginService.LoginAsync(loginDto, _mockHttpResponse.Object);

        // Assert
        Assert.Equal(StatusCodeType.Unauthorized, result.StatusCode);
        Assert.Equal("Login failed!", result.Data.Message);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "testuser", Password = "wrongPassword" };
        _mockPasswordVerifier.Setup(p => p.VerifyPasswordHash(loginDto.Password, It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = await _loginService.LoginAsync(loginDto, _mockHttpResponse.Object);

        // Assert
        Assert.Equal(StatusCodeType.Unauthorized, result.StatusCode);
        Assert.Equal("Login failed!", result.Data.Message);
    }
}