using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.UserPanelServices;

namespace RelationshipAnalysis.Test.Services;

public class NodeAdditionServiceTests
{
    private readonly INodeAdditionService _sut;
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public NodeAdditionServiceTests()
    {
        _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options);
            
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => _context);

        SeedDatabase();
        _serviceProvider = serviceCollection.BuildServiceProvider();

            
        _sut = new NodeAdditionService(_serviceProvider);
    }

    private void SeedDatabase()
    {
        _context.Add(new NodeCategory()
        {
            NodeCategoryName = "Account",
            NodeCategoryId = 1
        });
    }
    [Fact]
    public async Task AddNodes_ShouldReturnBadRequest_WhenUniqueHeaderIsInvalid()
    {
        // Arrange
        var filePath = "";
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        // Act
        var result = await _sut.AddNodes(new UploadNodeDto()
        {
            File = new FormFile(fileStream, 0, fileStream.Length, "file", Path.GetFileName(filePath)),
            NodeCategoryName = "Account",
            UniqueAttributeHeaderName = "SomeHeaderThatDoesntExist"
        });
        // Assert
        Assert.Equal(expected, result);
    }

    public async Task AddNodes_ShouldReturnBadRequest_WhenNodeCategoryIsInvalid()
    {
        var filePath = "";
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidNodeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        // Act
        var result = await _sut.AddNodes(new UploadNodeDto()
        {
            File = new FormFile(fileStream, 0, fileStream.Length, "file", Path.GetFileName(filePath)),
            NodeCategoryName = "SomeNodeCategoryThatDoesntExist",
            UniqueAttributeHeaderName = "AccountID"
        });
        // Assert
        Assert.Equal(expected, result);
    }

    
    public async Task AddNodes_ShouldReturnSucces_WhenNodeDtoIsValid()
    {
        var filePath = "";
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulNodeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
        // Act
        var result = await _sut.AddNodes(new UploadNodeDto()
        {
            File = new FormFile(fileStream, 0, fileStream.Length, "file", Path.GetFileName(filePath)),
            NodeCategoryName = "Account",
            UniqueAttributeHeaderName = "AccountID"
        });
        // Assert
        Assert.Equal(expected, result);
    }
}