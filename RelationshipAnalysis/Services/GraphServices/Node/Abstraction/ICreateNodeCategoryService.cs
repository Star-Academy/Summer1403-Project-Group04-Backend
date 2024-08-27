using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Node;

namespace RelationshipAnalysis.GraphServices.Node.Abstraction;

public interface ICreateNodeCategoryService
{
    Task<ActionResponse<MessageDto>> CreateNodeCategory(CreateNodeCategoryDto createNodeCategoryDto);
}