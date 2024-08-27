using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.GraphServices.Node.Abstraction;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.Abstraction;
using IContextNodesAdditionService = RelationshipAnalysis.Services.GraphServices.Node.Abstraction.IContextNodesAdditionService;

namespace RelationshipAnalysis.Services.GraphServices.Node;

public class ContextNodesAdditionService(IMessageResponseCreator responseCreator, 
ISingleNodeAdditionService singleNodeAdditionService) : IContextNodesAdditionService
{
    public async Task<ActionResponse<MessageDto>> AddToContext(string uniqueKeyHeaderName, ApplicationDbContext context, List<dynamic> objects, NodeCategory nodeCategory)
    {
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                foreach (var obj in objects)
                {
                    var dictObject = (IDictionary<string, object>)obj;
                    await singleNodeAdditionService.AddSingleNode(context, dictObject, uniqueKeyHeaderName,
                        nodeCategory.NodeCategoryId);
                }

                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return responseCreator.Create(StatusCodeType.BadRequest, e.Message);
            }
        }

        return responseCreator.Create(StatusCodeType.Success, Resources.SuccessfulNodeAdditionMessage);
    }
}