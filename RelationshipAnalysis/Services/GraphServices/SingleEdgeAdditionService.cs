using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class SingleEdgeAdditionService : ISingleEdgeAdditionService

{
    public Task AddSingleEdge(IDictionary<string, object> record, string uniqueHeaderName, string uniqueSourceHeaderName,
        string uniqueTargetHeaderName, int edgeCategoryId, int sourceNodeCategoryId, int targetNodeCategoryId)
    {
        throw new NotImplementedException();
    }
}