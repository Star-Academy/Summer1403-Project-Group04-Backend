namespace RelationshipAnalysis.GraphServices.Node.Abstraction;

public interface INodeCategoryReceiver
{
    Task<List<string>> GetAllNodeCategories();
}