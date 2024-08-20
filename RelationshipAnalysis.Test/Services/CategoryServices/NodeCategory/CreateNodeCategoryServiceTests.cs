﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Category;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Services.CategoryServices.NodeCategory;

namespace RelationshipAnalysis.Test.Services.CategoryServices.NodeCategory;

public class CreateNodeCategoryServiceTests
{
    private readonly CreateNodeCategoryService _sut;
    private readonly IServiceProvider _serviceProvider;

    public CreateNodeCategoryServiceTests()
    {
        var serviceCollection = new ServiceCollection();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        serviceCollection.AddScoped(_ => new ApplicationDbContext(options));

        _serviceProvider = serviceCollection.BuildServiceProvider();

        _sut = new(_serviceProvider);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.EdgeCategories.Add(new Models.Graph.EdgeCategory()
        {
            EdgeCategoryName = "ExistName"
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task CreateEdgeCategory_ShouldReturnBadRequest_WhenDtoIsNull()
    {
        // Arrange
        CreateNodeCategoryDto dto = null;

        // Act
        var result = await _sut.CreateNodeCategory(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.NullDtoErrorMessage, result.Data.Message);
    }

    [Fact]
    public async Task CreateEdgeCategory_ShouldReturnBadRequest_WhenCategoryNameIsNotUnique()
    {
        // Arrange
        var dto = new CreateNodeCategoryDto()
        {
            NodeCategoryName = "ExistName"
        };

        // Act
        var result = await _sut.CreateNodeCategory(dto);

        // Assert
        Assert.Equal(StatusCodeType.BadRequest, result.StatusCode);
        Assert.Equal(Resources.NotUniqueCategoryNameErrorMessage, result.Data.Message);
    }

    [Fact]
    public async Task CreateEdgeCategory_ShouldReturnSuccess_WhenDtoIsOk()
    {
        // Arrange
        var dto = new CreateNodeCategoryDto()
        {
            NodeCategoryName = "NotExistName"
        };
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Act
        var result = await _sut.CreateNodeCategory(dto);
        var categoty = context.NodeCategories.SingleOrDefault(c => c.NodeCategoryName == dto.NodeCategoryName);

        // Assert
        Assert.Equal(StatusCodeType.Success, result.StatusCode);
        Assert.Equal(Resources.SuccessfulCreateCategory, result.Data.Message);
        Assert.NotNull(categoty);
    }
}