using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Category;
using RelationshipAnalysis.Services.CategoryServices.EdgeCategory.Abstraction;

namespace RelationshipAnalysis.Services.CategoryServices.EdgeCategory;

public class EdgeCategoryReceiver : IEdgeCategoryReceiver
{
    public Task<List<string>> GetAllEdgeCategories()
    {
        throw new NotImplementedException();
    }
}