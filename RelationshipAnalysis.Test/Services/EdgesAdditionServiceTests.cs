using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Test;

public class EdgesAdditionServiceTests
{
    private IEdgesAdditionService _sut;
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public EdgesAdditionServiceTests()
    {
        _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options);
            
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
        _context.Add(new EdgeCategory()
        {
            EdgeCategoryName = "Transaction",
            EdgeCategoryId = 1
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenUniqueHeaderIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "SomeHeaderThatDoesntExist")
            .Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act 
        var result = await _sut.AddEdges(new UploadEdgeDto()
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "SomeHeaderThatDoesntExist",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }
    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenUniqueSourceHeaderIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "SomeHeaderThatDoesntExist")
            .Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act 
        var result = await _sut.AddEdges(new UploadEdgeDto()
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SomeHeaderThatDoesntExist",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }
    
    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenUniqueTargetHeaderIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidHeaderAttribute),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "SomeHeaderThatDoesntExist")
            .Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act 
        var result = await _sut.AddEdges(new UploadEdgeDto()
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "SomeHeaderThatDoesntExist"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }
    

    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenEdgeCategoryIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidEdgeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto()
        {
            File = fileToBeSend,
            EdgeCategoryName = "NotExistCategory",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }

    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenSourceNodeCategoryIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidSourceNodeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto()
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "NotExistAccount",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }
    [Fact]
    public async Task AddEdges_ShouldReturnBadRequest_WhenTargetNodeCategoryIsInvalid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.InvalidTargetNodeCategory),
            StatusCode = StatusCodeType.BadRequest
        };
        var csvContent = @"""SourceAcount"",""DestiantionAccount"",""Amount"",""Date"",""TransactionID"",""Type""
""6534454617"",""6039548046"",""500,000,000"",""1399/04/23"",""153348811341"",""پایا""
""6039548046"",""5287517379"",""100,000,000"",""1399/04/23"",""192524206627"",""پایا""";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto()
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "NotExistAccount",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }
    
    
    
    [Fact]
    public async Task AddNodes_ShouldReturnSuccess_WhenNodeDtoIsValid()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>()
        {
            Data = new MessageDto(Resources.SuccessfulNodeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
        var csvContent = @"""AccountID"",""CardID"",""IBAN""
""6534454617"",""6104335000000190"",""IR120778801496000000198""
""4000000028"",""6037699000000020"",""IR033880987114000000028""
";
        var fileToBeSend = CreateFileMock(csvContent);

        var validatorMock = NSubstitute.Substitute.For<ICsvValidatorService>();
        validatorMock.Validate(fileToBeSend, "AccountID").Returns(expected);
        var processorMock = NSubstitute.Substitute.For<ICsvProcessorService>();
        processorMock.ProcessCsvAsync(fileToBeSend).Returns(new List<dynamic>());
        var additionServiceMock = NSubstitute.Substitute.For<ISingleEdgeAdditionService>();
        _sut = new EdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock);
        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto()
        {
            File = fileToBeSend,
            EdgeCategoryName = "Transaction",
            UniqueKeyHeaderName = "TransactionID",
            SourceNodeCategoryName = "Account",
            TargetNodeCategoryName = "Account",
            SourceNodeHeaderName = "SourceAcount",
            TargetNodeHeaderName = "DestiantionAccount"
        });
        // Assert
        Assert.Equivalent(expected, result);
    }


    private IFormFile CreateFileMock(string csvContent)
    {
        var csvFileName = "test.csv";
        var fileMock = Substitute.For<IFormFile>();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(csvContent);
        writer.Flush();
        stream.Position = 0;

        fileMock.OpenReadStream().Returns(stream);
        fileMock.FileName.Returns(csvFileName);
        fileMock.Length.Returns(stream.Length);
        return fileMock;
    }
}