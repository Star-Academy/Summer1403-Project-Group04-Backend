using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Dto;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Enums;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class NodesAdditionService(
    IServiceProvider serviceProvider,
    ICsvValidatorService csvValidatorService,
    ICsvProcessorService csvProcessorService,
    ISingleNodeAdditionService singleNodeAdditionService) : INodesAdditionService
{
    public async Task<ActionResponse<MessageDto>> AddNodes(UploadNodeDto uploadNodeDto)
    {
        Console.WriteLine("______________________________________STAAAAAAAAAAART______________________________________");
        
        var t = Stopwatch.StartNew();
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var nodeCategory = await context.NodeCategories.SingleOrDefaultAsync(nc =>
            nc.NodeCategoryName == uploadNodeDto.NodeCategoryName);
        var file = uploadNodeDto.File;
        var uniqueHeader = uploadNodeDto.UniqueKeyHeaderName;

        if (nodeCategory == null)
            return BadRequestResult(Resources.InvalidNodeCategory);

        var validationResult = csvValidatorService.Validate(file, uniqueHeader);
        if (validationResult.StatusCode == StatusCodeType.BadRequest)
            return validationResult;

        var objects = await csvProcessorService.ProcessCsvAsync(file);

        // foreach (var key in ((IDictionary<string, object>)objects[0]).Keys)
        // {
        //     var newNodeAttribute = new NodeAttribute()
        //     {
        //         NodeAttributeName = key
        //     };
        //     await context.NodeAttributes.AddAsync(newNodeAttribute);
        //     await context.SaveChangesAsync();
        // }
        
        // await using (var transaction = await context.Database.BeginTransactionAsync())
        // {
        //     try
        //     {
                foreach (var obj in objects)
                {
                    await singleNodeAdditionService.AddSingleNode((IDictionary<string, object>)obj, uniqueHeader,
                        nodeCategory.NodeCategoryId);
                }

                await context.SaveChangesAsync();
        //         await transaction.CommitAsync();
        //     }
        //     catch (Exception e)
        //     {
        //         await transaction.RollbackAsync();
        //         return BadRequestResult(e.Message);
        //     }
        // }
        t.Stop();
        Console.WriteLine("______________________________________"+t.ElapsedMilliseconds.ToString()+"______________________________________");
        return SuccessResult();
    }


    private ActionResponse<MessageDto> BadRequestResult(string message)
    {
        return new ActionResponse<MessageDto>
        {
            Data = new MessageDto(message),
            StatusCode = StatusCodeType.BadRequest
        };
    }

    private ActionResponse<MessageDto> SuccessResult()
    {
        return new ActionResponse<MessageDto>
        {
            Data = new MessageDto(Resources.SuccessfulNodeAdditionMessage),
            StatusCode = StatusCodeType.Success
        };
    }
}