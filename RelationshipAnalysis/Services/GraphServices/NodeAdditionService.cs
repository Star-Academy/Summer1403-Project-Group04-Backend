using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class NodeAdditionService(IServiceProvider serviceProvider) : INodeAdditionService
{
    public Task<ActionResponse<MessageDto>> AddNodes(UploadNodeDto uploadNodeDto)
    {
        throw new NotImplementedException();
    }
}