namespace RelationshipAnalysis.GraphServices.Edge.Abstraction;

public interface IEdgeCategoryReceiver
{
    Task<List<string>> GetAllEdgeCategories();
}