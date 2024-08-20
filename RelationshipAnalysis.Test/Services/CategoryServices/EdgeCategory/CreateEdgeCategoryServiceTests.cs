using RelationshipAnalysis.Dto.Category;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.CategoryServices.EdgeCategory;

namespace RelationshipAnalysis.Test.Services.CategoryServices.EdgeCategory;

public class CreateEdgeCategoryServiceTests
{
    private readonly CreateEdgeCategoryService _sut;

    public CreateEdgeCategoryServiceTests()
    {
        _sut = new();
    }

    [Fact]
    public async Task CreateEdgeCategory_ShouldReturnBadRequest_WhenDtoIsNull()
    {
        // Arrange
        CreateEdgeCategoryDto dto = null;

        // Act
        var result = await _sut.CreateEdgeCategory(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.NullDtoErrorMessage, result.Data.Message);
    }
    
    [Fact]
    public async Task CreateEdgeCategory_ShouldReturnBadRequest_WhenCategoryNameIsNotUnique()
    {
        // Arrange
        var dto = new CreateEdgeCategoryDto()
        {
            EdgeCategoryName = "ExistName"
        };

        // Act
        var result = await _sut.CreateEdgeCategory(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.NotUniqueCategoryNameErrorMessage, result.Data.Message);
    }
    
    [Fact]
    public async Task CreateEdgeCategory_ShouldReturnSuccess_WhenDtoIsOk()
    {
        // Arrange
        var dto = new CreateEdgeCategoryDto()
        {
            EdgeCategoryName = "NotExistName"
        };

        // Act
        var result = await _sut.CreateEdgeCategory(dto);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Equal(Resources.SuccessfulCreateCategory, result.Data.Message);
    }
}