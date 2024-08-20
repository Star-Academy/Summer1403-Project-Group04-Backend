using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Category;
using RelationshipAnalysis.Services.CategoryServices.NodeCategory.Abstraction;

namespace RelationshipAnalysis.Services.CategoryServices.NodeCategory;

public class CreateNodeCategoryService : ICreateNodeCategoryService 
{
    public Task<ActionResponse<MessageDto>> CreateNodeCategory(CreateNodeCategoryDto createNodeCategoryDto)
    {
        throw new NotImplementedException();
    }
}