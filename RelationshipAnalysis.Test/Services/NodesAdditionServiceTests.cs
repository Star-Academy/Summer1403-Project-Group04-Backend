using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.UserPanelServices;

namespace RelationshipAnalysis.Test.Services;

public class NodesAdditionServiceTests
{
    private INodesAdditionService _sut;
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public NodesAdditionServiceTests()
    {
        _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options);
            
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped(_ => _context);

        SeedDatabase();
        _serviceProvider = serviceCollection.BuildServiceProvider();

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
        var fileToBeSend = new FormFile(fileStream, 0, fileStream.Length, "file", Path.GetFileName(filePath));
        
        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "SomeHeaderThatDoesntExist")
            .Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleNodeAdditionService>();
        _sut = new NodesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddNodes(new UploadNodeDto()
        {
            File = fileToBeSend,
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

        var fileToBeSend = new FormFile(fileStream, 0, fileStream.Length, "file", Path.GetFileName(filePath));
        
        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleNodeAdditionService>();
        _sut = new NodesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddNodes(new UploadNodeDto()
        {
            File = fileToBeSend,
            NodeCategoryName = "SomeNodeCategoryThatDoesntExist",
            UniqueAttributeHeaderName = "AccountID"
        });
        // Assert
        Assert.Equal(expected, result);
    }

    
    public async Task AddNodes_ShouldReturnSucces_WhenNodeDtoIsValid()
    {
        // Arrange
        var filePath = "";
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulNodeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
        
        
        var fileToBeSend = new FormFile(fileStream, 0, fileStream.Length, "file", Path.GetFileName(filePath));
        
        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "AccountID").Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        processorMock.ProcessCsvAsync(fileToBeSend).Returns(new List<dynamic>());
        var additionServiceMock = NSubstitute.Substitute.For<ISingleNodeAdditionService>();
        _sut = new NodesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        
        // Act
        var result = await _sut.AddNodes(new UploadNodeDto()
        {
            File = fileToBeSend,
            NodeCategoryName = "Account",
            UniqueAttributeHeaderName = "AccountID"
        });
        // Assert
        Assert.Equal(expected, result);
    }
}