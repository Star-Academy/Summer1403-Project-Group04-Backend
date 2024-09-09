using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;

namespace RelationshipAnalysis.Settings.DbContext;

public static class DbContextExtensions
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration["CONNECTION_STRING"]).UseLazyLoadingProxies());
        
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.LastNode = dbContext.Nodes?.Count() ?? 0;
        dbContext.LastNodeAttribute = dbContext.NodeAttributes?.Count() ?? 0;
        dbContext.LastEdge = dbContext.Edges?.Count() ?? 0;
        dbContext.LastEdgeAttribute = dbContext.EdgeAttributes?.Count() ?? 0;

        return services;
    }
}