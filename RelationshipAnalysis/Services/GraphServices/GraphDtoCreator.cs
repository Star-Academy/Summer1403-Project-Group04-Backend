using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using Node = AngleSharp.Dom.Node;

namespace RelationshipAnalysis.Services.GraphServices;

public class GraphDtoCreator : IGraphDtoCreator
{
    
    public GraphDto CreateResultGraphDto(List<Models.Graph.Node> contextNodes, List<Models.Graph.Edge> contextEdges)
    {
        if (contextEdges == null || contextNodes == null) throw new ArgumentNullException();
        var resultGraphDto = new GraphDto();
        contextNodes.ForEach(n => resultGraphDto.Nodes.Add(new NodeDto
        {
            id = n.NodeId.ToString(),
            label = $"{n.NodeCategory.NodeCategoryName}/{n.NodeUniqueString}"
        }));
        contextEdges.ForEach(e => resultGraphDto.Edges.Add(new EdgeDto
        {
            id = e.EdgeId.ToString(),
            source = e.EdgeSourceNodeId.ToString(),
            target = e.EdgeDestinationNodeId.ToString()
        }));
        return resultGraphDto;
    }
}