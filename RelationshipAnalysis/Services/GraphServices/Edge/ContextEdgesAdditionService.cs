using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class ContextEdgesAdditionService(ISingleEdgeAdditionService) : IContextEdgesAdditionService
{
    public async Task<ActionResponse<MessageDto>> AddToContext(ApplicationDbContext context, EdgeCategory edgeCategory, NodeCategory sourceCategory,
        NodeCategory targetCategory, List<dynamic> objects, UploadEdgeDto uploadEdgeDto)
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