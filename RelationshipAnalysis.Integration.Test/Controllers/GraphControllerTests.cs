using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Moq;
using Newtonsoft.Json;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Models.Auth;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.UserPanelServices.Abstraction.AuthServices;
using RelationshipAnalysis.Settings.JWT;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace RelationshipAnalysis.Integration.Test.Controllers;

public class GraphControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    
    private readonly HttpClient _client;
    private readonly JwtSettings _jwtSettings;

    public GraphControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _jwtSettings = new JwtSettings
        {
            Key = "kajbdiuhdqhpjQE89HBSDJIABFCIWSGF89GW3EJFBWEIUBCZNMXCJNLZDKNJKSNJKFBIGW3EASHHDUIASZGCUI",
            ExpireMinutes = 60
        };
    }

    [Fact]
    public async Task GetGraph_ShouldReturnGraph_WhenUserIsAuthorized()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/graph/getgraph");
        Mock<IOptions<JwtSettings>> mockJwtSettings = new();
        mockJwtSettings.Setup(m => m.Value).Returns(_jwtSettings);
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            UserRoles = new List<UserRole>() { new UserRole() { Role = new Role() { Name = "admin" } } }

        };

        var token = new JwtTokenGenerator(mockJwtSettings.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        
        var nodeCategory1 = new NodeCategory { NodeCategoryName = "Account" };
        var nodeCategory2 = new NodeCategory { NodeCategoryName = "Person" };
        
        var node1 = new Node 
        {
            NodeUniqueString = "Node1",
            NodeCategory = nodeCategory1,
            NodeCategoryId = nodeCategory1.NodeCategoryId
        };

        var node2 = new Node
        {
            NodeUniqueString = "Node2",
            NodeCategory = nodeCategory2,
            NodeCategoryId = nodeCategory2.NodeCategoryId
        };

        var edgeCategory = new EdgeCategory { EdgeCategoryName = "Transaction"};



        var expectedNodes = new List<NodeDto>()
        {
            new NodeDto()
            {
                id = node1.NodeId.ToString(),
                label = $"{node1.NodeCategory.NodeCategoryName}/{node1.NodeUniqueString}"
            },
            new NodeDto()
            {
                id = node2.NodeId.ToString(),
                label = $"{node2.NodeCategory.NodeCategoryName}/{node2.NodeUniqueString}"
            }
        };
        
        var edge = new Edge
        {
            EdgeSourceNodeId = node1.NodeId,
            EdgeDestinationNodeId = node2.NodeId,
            EdgeCategory = edgeCategory,
            EdgeCategoryId = edgeCategory.EdgeCategoryId,
            EdgeUniqueString = "Edge1"
        };


        var expectedEdges = new List<EdgeDto>()
        {
            new EdgeDto()
            {
                id = edge.EdgeId.ToString(),
                source = node1.NodeId.ToString(),
                target = node2.NodeId.ToString()
            }
        };
        
        
        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<GraphDto>();
        Assert.Equivalent(responseData.nodes, expectedNodes);
        Assert.Equivalent(responseData.edges, expectedEdges);

    }
    [Fact]
    public async Task UploadNode_ShouldReturnBadRequest_WhenNoFileUploaded()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "api/graph/uploadnode");
        
        var filePath = "";
        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

        var formDataContent = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        formDataContent.Add(fileContent, "File", Path.GetFileName(filePath));
        
        formDataContent.Add(new StringContent("Account"), "NodeCategoryName");
        formDataContent.Add(new StringContent("AccountID"), "UniqueAttributeHeaderName");
        
        request.Content = formDataContent;
        
        Mock<IOptions<JwtSettings>> jwtSettingsMock = new();
        jwtSettingsMock.Setup(m => m.Value).Returns(_jwtSettings);
        
        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            UserRoles = new List<UserRole>() { new UserRole() { Role = new Role() { Name = "admin" } } }

        };

        var token = new JwtTokenGenerator(jwtSettingsMock.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        
        // Act
        var response = await _client.SendAsync(request);
        
        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.Equal(Resources.NoFileUploadedMessage, response.Content.ReadFromJsonAsync<MessageDto>().Result.Message);
    }

    public async Task UploadNode_ShouldReturnSuccess_WhenDtoIsValid()
    {

        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "api/graph/uploadnode");
        Mock<IOptions<JwtSettings>> jwtSettingsMock = new();
        jwtSettingsMock.Setup(m => m.Value).Returns(_jwtSettings);

        var user = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "74b2c5bd3a8de69c8c7c643e8b5c49d6552dc636aeb0995aff6f01a1f661a979",
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            UserRoles = new List<UserRole>() { new UserRole() { Role = new Role() { Name = "admin" } } }

        };

        var token = new JwtTokenGenerator(jwtSettingsMock.Object).GenerateJwtToken(user);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(400, (int)response.StatusCode);
        Assert.Equal(Resources.NoFileUploadedMessage, response.Content.ReadFromJsonAsync<MessageDto>().Result.Message);
    }
}