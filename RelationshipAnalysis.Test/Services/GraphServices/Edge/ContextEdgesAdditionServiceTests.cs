

using Moq;
using NSubstitute;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;
using RelationshipAnalysis.Test;

public class ContextEdgesAdditionServiceTests()
{
    private IContextEdgesAdditionService _sut;
    public ContextEdgesAdditionServiceTests()
    {
        
    }
    
    
    [Fact]
    public async Task AddToContext_ShouldReturnBadRequestAndRollBack_WhenDbFailsToAddData()
    {
        // Arrange
        var expected = new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.SuccessfulEdgeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
        
        var additionServiceMock = new Mock<IContextEdgesAdditionService>();

        additionServiceMock
            .Setup(service => service.AddToContext(
                It.IsAny<ApplicationDbContext>(),
                It.IsAny<EdgeCategory>(),
                It.IsAny<NodeCategory>(),
                It.IsAny<NodeCategory>(),
                It.IsAny<List<dynamic>>(),
                It.IsAny<UploadEdgeDto>()
            ))
            .Throws(new Exception("Custom exception message"));
        _sut = new ContextEdgesAdditionService(_serviceProvider, validatorMock, processorMock, additionServiceMock.Object, new MessageResponseCreator());

        // Act
        var result = await _sut.AddEdges(new UploadEdgeDto
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
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        Assert.Equal(0, context.Nodes.Count());
        Assert.Equal("Custom exception message", result.Data.Message);
    }
}