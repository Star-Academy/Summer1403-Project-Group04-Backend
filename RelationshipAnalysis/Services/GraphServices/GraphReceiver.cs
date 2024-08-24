using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Dto.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class GraphReceiver(IServiceProvider serviceProvider) : IGraphReceiver
{
    public async Task<GraphDto> GetGraph()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var contextNodes = await context.Nodes.Include(node => node.NodeCategory).ToListAsync();
        var contextEdges = await context.Edges.ToListAsync();

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