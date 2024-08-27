using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;

namespace RelationshipAnalysis.GraphServices.Edge.Abstraction;

public interface ICreateEdgeCategoryService
{
    Task<ActionResponse<MessageDto>> CreateEdgeCategory(CreateEdgeCategoryDto createEdgeCategoryDto);
}