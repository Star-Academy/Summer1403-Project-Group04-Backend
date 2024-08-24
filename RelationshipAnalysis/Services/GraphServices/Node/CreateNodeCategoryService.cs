using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Node.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Node;

public class CreateNodeCategoryService(IServiceProvider serviceProvider) : ICreateNodeCategoryService
{
    public async Task<ActionResponse<MessageDto>> CreateNodeCategory(CreateNodeCategoryDto createNodeCategoryDto)
    {
        if (createNodeCategoryDto is null) return BadRequestResult(Resources.NullDtoErrorMessage);
        if (IsNotUniqueCategoryName(createNodeCategoryDto))
            return BadRequestResult(Resources.NotUniqueCategoryNameErrorMessage);
        await AddCategory(createNodeCategoryDto);
        return SuccessfulResult(Resources.SuccessfulCreateCategory);
    }

    private ActionResponse<MessageDto> SuccessfulResult(string message)
    {
        return new ActionResponse<MessageDto>
        {
            Data = new MessageDto(message),
            StatusCode = StatusCodeType.Success
        };
    }

    private async Task AddCategory(CreateNodeCategoryDto createNodeCategoryDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.NodeCategories.AddAsync(new NodeCategory
        {
            NodeCategoryName = createNodeCategoryDto.NodeCategoryName
        });
        await context.SaveChangesAsync();
    }

    private bool IsNotUniqueCategoryName(CreateNodeCategoryDto dto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return context.NodeCategories.Any(c => c.NodeCategoryName == dto.NodeCategoryName);
    }

    private ActionResponse<MessageDto> BadRequestResult(string message)
    {
        return new ActionResponse<MessageDto>
        {
            Data = new MessageDto(message),
            StatusCode = StatusCodeType.BadRequest
        };
    }
}