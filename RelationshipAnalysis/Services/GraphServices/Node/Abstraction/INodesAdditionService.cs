using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Node;

namespace RelationshipAnalysis.GraphServices.Node.Abstraction;

public interface INodesAdditionService
{
    Task<ActionResponse<MessageDto>> AddNodes(UploadNodeDto uploadNodeDto);
}