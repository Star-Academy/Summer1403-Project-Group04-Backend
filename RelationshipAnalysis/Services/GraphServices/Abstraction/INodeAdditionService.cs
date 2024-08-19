using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface INodeAdditionService
{
    Task<ActionResponse<MessageDto>> AddNodes(UploadNodeDto uploadNodeDto);
}