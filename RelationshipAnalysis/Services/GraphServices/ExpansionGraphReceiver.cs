using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class ExpansionGraphReceiver(IServiceProvider serviceProvider, IGraphDtoCreator graphDtoCreator, IResponseMessageCreator responseCreator) : IExpansionGraphReceiver
{
    public async Task<ActionResponse<GraphDto>> GetExpansionGraph(ExpansionDto expansionDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var inputNodes = await GetInputNodes(expansionDto, context);
        var outputNodes = await GetOutputNodes(expansionDto, context);
        var validEdges = await GetValidEdges(expansionDto, context, inputNodes, outputNodes);
        
        var resultData = graphDtoCreator.CreateResultGraphDto(inputNodes.Union(outputNodes).ToList(), validEdges);
        return responseCreator.Create()
    }

    private async Task<List<Edge>> GetValidEdges(ExpansionDto expansionDto, ApplicationDbContext context, List<Node> inputNodes,
        List<Node> outputNodes)
    {
        var validEdges = await context.Edges.Where(e =>
            e.EdgeCategoryId == expansionDto.EdgeCategoryId &&
            inputNodes.Contains(e.NodeSource) &&
            outputNodes.Contains(e.NodeDestination)).ToListAsync();
        return validEdges;
    }

    private async Task<List<Node>> GetOutputNodes(ExpansionDto expansionDto, ApplicationDbContext context)
    {
        var outputNodes =
            await context.Nodes.Where(n => n.NodeCategoryId == expansionDto.TargetCategoryId).ToListAsync();
        return outputNodes;
    }

    private async Task<List<Node>> GetInputNodes(ExpansionDto expansionDto, ApplicationDbContext context)
    {
        var inputNodes =
            await context.Nodes.Where(n => n.NodeCategoryId == expansionDto.SourceCategoryId).ToListAsync();
        return inputNodes;
    }
}