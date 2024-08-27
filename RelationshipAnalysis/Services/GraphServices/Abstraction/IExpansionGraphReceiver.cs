using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface IExpansionGraphReceiver
{
    Task<ActionResponse<GraphDto>> GetExpansionGraph(ExpansionDto expansionDto);
}