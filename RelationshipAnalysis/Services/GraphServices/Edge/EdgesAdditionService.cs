using Microsoft.EntityFrameworkCore;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph.Edge;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph.Edge;
using RelationshipAnalysis.Models.Graph.Node;
using RelationshipAnalysis.Services.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Edge.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices.Edge;

public class EdgesAdditionService(
    IServiceProvider serviceProvider,
    ICsvValidatorService csvValidatorService,
    ICsvProcessorService csvProcessorService,
    ISingleEdgeAdditionService singleEdgeAdditionService,
    IMessageResponseCreator responseCreator) : IEdgesAdditionService
{
    public async Task<ActionResponse<MessageDto>> AddEdges(UploadEdgeDto uploadEdgeDto)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var targetCategory = await GetTargetCategory(uploadEdgeDto, context);
        var sourceCategory = await GetSourceCategory(uploadEdgeDto, context);
        var edgeCategory = await GetEdgeCategory(uploadEdgeDto, context);
        
        var nullCheckResponse = CheckForNullValues(edgeCategory, sourceCategory, targetCategory);
        if (nullCheckResponse.StatusCode == StatusCodeType.BadRequest)
        {
            return nullCheckResponse;
        }

        var validationResult = csvValidatorService.Validate(uploadEdgeDto.File, uploadEdgeDto.UniqueKeyHeaderName, uploadEdgeDto.SourceNodeHeaderName, uploadEdgeDto.TargetNodeHeaderName);
        if (validationResult.StatusCode == StatusCodeType.BadRequest)
        {
            return validationResult;
        }

        var objects = await csvProcessorService.ProcessCsvAsync(uploadEdgeDto.File);

        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                foreach (var obj in objects)
                {
                    var dictObject = (IDictionary<string, object>)obj;
                    await singleEdgeAdditionService.AddSingleEdge(context, dictObject,
                        uploadEdgeDto.UniqueKeyHeaderName, 
                        uploadEdgeDto.SourceNodeHeaderName,
                        uploadEdgeDto.TargetNodeHeaderName, 
                        edgeCategory.EdgeCategoryId,
                        sourceCategory.NodeCategoryId,
                        targetCategory.NodeCategoryId);
                }
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return responseCreator.Create(StatusCodeType.BadRequest, e.Message);
            }
        }

        return responseCreator.Create(StatusCodeType.Success, Resources.SuccessfulEdgeAdditionMessage);
    }

    private ActionResponse<MessageDto> CheckForNullValues(EdgeCategory? edgeCategory, NodeCategory? sourceCategory, NodeCategory? targetCategory)
    {
        if (edgeCategory == null)
        {
            return responseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidEdgeCategory);
        }

        if (sourceCategory == null)
        {
            return responseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidSourceNodeCategory);
        }

        if (targetCategory == null)
        {
            return responseCreator.Create(StatusCodeType.BadRequest, Resources.InvalidTargetNodeCategory);
        }

        return responseCreator.Create(StatusCodeType.Success, string.Empty);
    }

    private async Task<NodeCategory?> GetTargetCategory(UploadEdgeDto uploadEdgeDto, ApplicationDbContext context)
    {
        var targetNodeCategory = await context.NodeCategories.SingleOrDefaultAsync(nc =>
            nc.NodeCategoryName == uploadEdgeDto.TargetNodeCategoryName);
        return targetNodeCategory;
    }

    private async Task<NodeCategory?> GetSourceCategory(UploadEdgeDto uploadEdgeDto, ApplicationDbContext context)
    {
        var sourceNodeCategory = await context.NodeCategories.SingleOrDefaultAsync(nc =>
            nc.NodeCategoryName == uploadEdgeDto.SourceNodeCategoryName);
        return sourceNodeCategory;
    }

    private async Task<EdgeCategory?> GetEdgeCategory(UploadEdgeDto uploadEdgeDto, ApplicationDbContext context)
    {
        var edgeCategory = await context.EdgeCategories.SingleOrDefaultAsync(ec =>
            ec.EdgeCategoryName == uploadEdgeDto.EdgeCategoryName);
        return edgeCategory;
    }
}