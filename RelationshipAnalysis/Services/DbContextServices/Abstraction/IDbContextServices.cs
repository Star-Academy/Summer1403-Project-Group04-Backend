using RelationshipAnalysis.Context;

namespace RelationshipAnalysis.Services.Abstraction;

public interface IDbContextServices
{
    ApplicationDbContext GetContext();
}