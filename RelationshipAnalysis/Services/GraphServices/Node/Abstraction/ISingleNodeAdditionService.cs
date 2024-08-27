using RelationshipAnalysis.Context;

namespace RelationshipAnalysis.GraphServices.Node.Abstraction;

public interface ISingleNodeAdditionService
{
    Task AddSingleNode(ApplicationDbContext context, IDictionary<string, object> record, string uniqueHeaderName,
        int nodeCategoryId);
}