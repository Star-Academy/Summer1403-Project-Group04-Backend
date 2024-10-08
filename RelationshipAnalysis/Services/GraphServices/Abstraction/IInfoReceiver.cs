using RelationshipAnalysis.Dto;

namespace RelationshipAnalysis.Services.GraphServices.Abstraction;

public interface IInfoReceiver
{
    Task<ActionResponse<IDictionary<string, string>>> GetInfo(int id);
}