using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class SingleEdgeAdditionService : ISingleEdgeAdditionService

{
    public async Task AddSingleEdge(ApplicationDbContext context, IDictionary<string, object> record,
        string uniqueHeaderName,
        string uniqueSourceHeaderName,
        string uniqueTargetHeaderName, int edgeCategoryId, int sourceNodeCategoryId, int targetNodeCategoryId)
    {
        if (((string)record[uniqueHeaderName]).IsNullOrEmpty())
        {
            throw new Exception(Resources.FailedAddRecordsMessage);
        }
        if (((string)record[uniqueSourceHeaderName]).IsNullOrEmpty())
        {
            throw new Exception(Resources.FailedAddRecordsMessage);
        }
        if (((string)record[uniqueTargetHeaderName]).IsNullOrEmpty())
        {
            throw new Exception(Resources.FailedAddRecordsMessage);
        }

        var source = await GetSourceNode(context, record, uniqueSourceHeaderName, sourceNodeCategoryId);
        if (source == null)
        {
            throw new Exception(Resources.FailedAddRecordsMessage);
        }

        var target = await GetTargetNode(context, record, uniqueTargetHeaderName, targetNodeCategoryId);
        if (target == null)
        {
            throw new Exception(Resources.FailedAddRecordsMessage);
        }

        var newEdge = await GetNewEdge(context, edgeCategoryId, uniqueHeaderName, source, target, record);
        if (newEdge.EdgeSourceNodeId != source.NodeId || newEdge.EdgeDestinationNodeId != target.NodeId)
        {
            throw new Exception(Resources.FailedAddRecordsMessage);
        }

        foreach (var kvp in record)
        {
            if (kvp.Key != uniqueHeaderName)
            {
                var newEdgeAttribute = await context.EdgeAttributes.SingleOrDefaultAsync(na =>
                    na.EdgeAttributeName == kvp.Key);
                if (newEdgeAttribute == null)
                {
                    newEdgeAttribute = new EdgeAttribute
                    {
                        EdgeAttributeName = kvp.Key
                    };
                    await context.AddAsync(newEdgeAttribute);
                    await context.SaveChangesAsync();
                }

                var value = await context.EdgeValues.SingleOrDefaultAsync(nv =>
                    nv.EdgeAttributeId == newEdgeAttribute.EdgeAttributeId &&
                    nv.EdgeId == newEdge.EdgeId);

                if (value != null) throw new Exception(Resources.FailedAddRecordsMessage);

                var newEdgeValue = new EdgeValue
                {
                    EdgeAttributeId = newEdgeAttribute.EdgeAttributeId,
                    ValueData = kvp.Value.ToString(),
                    EdgeId = newEdge.EdgeId
                };

                await context.AddAsync(newEdgeValue);
                await context.SaveChangesAsync();
            }
        }
    }

    private async Task<Models.Graph.Edge.Edge> GetNewEdge(ApplicationDbContext context, int edgeCategoryId, string uniqueHeaderName, Models.Graph.Node.Node source, Models.Graph.Node.Node target, IDictionary<string, object> record)
    {
        var newEdge = await context.Edges.SingleOrDefaultAsync(e =>
            e.EdgeCategoryId == edgeCategoryId
            && e.EdgeUniqueString == (string)record[uniqueHeaderName]);
        if (newEdge == null)
        {
            newEdge = new Models.Graph.Edge.Edge
            {
                EdgeUniqueString = (string)record[uniqueHeaderName],
                EdgeSourceNodeId = source.NodeId,
                EdgeDestinationNodeId = target.NodeId,
                EdgeCategoryId = edgeCategoryId
            };
            await context.AddAsync(newEdge);
            await context.SaveChangesAsync();
        }

        return newEdge;
    }

    private async Task<Models.Graph.Node.Node?> GetTargetNode(ApplicationDbContext context, IDictionary<string, object> record, string uniqueTargetHeaderName,
        int targetNodeCategoryId)
    {
        var target =
            await context.Nodes.SingleOrDefaultAsync(n => n.NodeUniqueString == (string)record[uniqueTargetHeaderName]
                                                          && n.NodeCategoryId == targetNodeCategoryId);
        return target;
    }

    private async Task<Models.Graph.Node.Node?> GetSourceNode(ApplicationDbContext context, IDictionary<string, object> record, string uniqueSourceHeaderName,
        int sourceNodeCategoryId)
    {
        var source =
            await context.Nodes.SingleOrDefaultAsync(n => n.NodeUniqueString == (string)record[uniqueSourceHeaderName]
                                                          && n.NodeCategoryId == sourceNodeCategoryId);
        return source;
    }
}