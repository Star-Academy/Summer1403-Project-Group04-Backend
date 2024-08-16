
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Test.Services
{
    public class UserUpdateInfoServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ICookieSetter _cookieSetter;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserReceiver _userReceiver;
        private readonly UserUpdateInfoService _sut;

        public UserUpdateInfoServiceTests()
        {
            _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options);

            _mapper = Substitute.For<IMapper>();
            _cookieSetter = Substitute.For<ICookieSetter>();
            _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
            _userReceiver = Substitute.For<IUserReceiver>();
            _sut = new UserUpdateInfoService(_context, _userReceiver, _mapper, _cookieSetter, _jwtTokenGenerator);
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Users.Add(new User
            {
                Id = 1,
                Username = "ExistingUser",
                Email = "user@example.com",
                FirstName = "",
                LastName = "",
                PasswordHash = "HashedPassword"
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsNotFound_WhenUserIsNull()
        {
            // Arrange
            User user = null;
            var userUpdateInfoDto = new UserUpdateInfoDto();
            var response = Substitute.For<HttpResponse>();

            // Act
            var result = await _sut.UpdateUserAsync(user, userUpdateInfoDto, response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
            Assert.Equal(Resources.UserNotFoundMessage, result.Data.Message);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsSuccess_WhenUserIsUpdated()
        {
            // Arrange
            var user = await _context.Users.FindAsync(1);
            var userUpdateInfoDto = new UserUpdateInfoDto
            {
                Username = "UpdatedUserName",
                Email = "updated@example.com"
            };
            var resultUser = new User()
            {
                Id = 1,
                Username = "UpdatedUserName",
                Email = "updated@example.com",
                FirstName = "",
                LastName = "",
                PasswordHash = "HashedPassword"
            };
            _mapper.Map<User>(userUpdateInfoDto).Returns(resultUser);
            
            var response = Substitute.For<HttpResponse>();
            
            // Act
            var result = await _sut.UpdateUserAsync(user, userUpdateInfoDto, response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodeType.Success, result.StatusCode);
            Assert.Equal(Resources.SuccessfulUpdateUserMessage, result.Data.Message);
            Assert.Equal("UpdatedUserName", user.Username);
            Assert.Equal("updated@example.com", user.Email);
        }

        [Fact]
        public async Task UpdateUserAsync_SetsJwtCookie_WhenUserIsUpdated()
        {
            // Arrange
            var user = await _context.Users.FindAsync(1);
            var userUpdateInfoDto = new UserUpdateInfoDto();
            var response = Substitute.For<HttpResponse>();
            var token = "fake-jwt-token";
            _jwtTokenGenerator.GenerateJwtToken(user).Returns(token);

            // Act
            await _sut.UpdateUserAsync(user, userUpdateInfoDto, response);

            // Assert
            _cookieSetter.Received(1).SetCookie(response, token);
        }
    }
}
