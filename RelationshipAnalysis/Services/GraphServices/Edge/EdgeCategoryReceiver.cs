using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.GraphServices.Edge;

public class EdgeCategoryReceiver(IServiceProvider serviceProvider) : IEdgeCategoryReceiver
{
    public async Task<List<string>> GetAllEdgeCategories()
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.EdgeCategories.Select(e => e.EdgeCategoryName).ToListAsync();
    }
}