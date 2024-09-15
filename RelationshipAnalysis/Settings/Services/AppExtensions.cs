using RelationshipAnalysis.Context;

namespace RelationshipAnalysis.Settings.Services;

public static class AppExtensions
{
    public static void SetContextCounts(this WebApplication app)
    {
        var provider = app.Services.GetService<IServiceProvider>();
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.LastNode = context.Nodes.Count();
        context.LastEdge = context.Edges.Count();
        context.LastEdgeAttribute = context.EdgeAttributes.Count();
        context.LastNodeAttribute = context.NodeAttributes.Count();
    }
}