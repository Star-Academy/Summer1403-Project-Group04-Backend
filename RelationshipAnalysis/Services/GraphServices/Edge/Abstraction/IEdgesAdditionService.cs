using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;

namespace RelationshipAnalysis.GraphServices.Edge.Abstraction;

public interface IEdgesAdditionService
{
    Task<ActionResponse<MessageDto>> AddEdges(UploadEdgeDto uploadEdgeDto);
}