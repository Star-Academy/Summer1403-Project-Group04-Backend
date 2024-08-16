using AutoMapper;
using NSubstitute;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.Abstractions;

namespace RelationshipAnalysis.Test.Services
{
    public class UserInfoServiceTests
    {
        private readonly IUserReceiver _userReceiver;
        private readonly IUserRolesReceiver _rolesReceiver;
        private readonly IMapper _mapper;
        private readonly UserInfoService _service;

        public UserInfoServiceTests()
        {
            _userReceiver = Substitute.For<IUserReceiver>();
            _rolesReceiver = Substitute.For<IUserRolesReceiver>();
            _mapper = Substitute.For<IMapper>();
            _service = new UserInfoService(_userReceiver, _rolesReceiver, _mapper);
        }

        [Fact]
        public async Task GetUserAsync_ReturnsNotFound_WhenUserIsNull()
        {
            // Arrange
            User user = null;

            // Act
            var result = await _service.GetUserAsync(user);

            // Assert
            Assert.Equal(StatusCodeType.NotFound, result.StatusCode);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetUserAsync_ReturnsSuccess_WhenUserIsNotNull_()
        {
            // Arrange
            var user = new User { Username = "Admin" };
            var resultData = new UserOutputInfoDto() {Username = "Admin"};
            var roles = new List<string> { "Admin", "User" };

            _mapper.Map<UserOutputInfoDto>(user).Returns(resultData);

            _rolesReceiver.ReceiveRoles(user.Id).Returns(roles);

            // Act
            var result = await _service.GetUserAsync(user);

            // Assert
            Assert.Equal(StatusCodeType.Success, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(user.Username, resultData.Username);
            Assert.Equal(roles, result.Data.Roles);
        }
    }
}
