using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Models.Graph;
using Node = AngleSharp.Dom.Node;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface IGraphDtoCreator
{
    GraphDto CreateResultGraphDto(List<Models.Graph.Node> contextNodes, List<Models.Graph.Edge> contextEdges);
}