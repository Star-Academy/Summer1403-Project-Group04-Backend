using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelationshipAnalysis.Context;
using RelationshipAnalysis.Models.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Services.GraphServices;

public class SingleNodeAdditionService(IServiceProvider serviceProvider) : ISingleNodeAdditionService
{
    
    public async Task AddSingleNode(IDictionary<string, object> record, string uniqueHeaderName, int nodeCategoryId)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var t = Stopwatch.StartNew();
        
        if (((string)record[uniqueHeaderName]).IsNullOrEmpty())
        {
            throw new Exception(Resources.FailedAddRecordsMessage);
        }

        Console.WriteLine("AddNode_____________________________________________________________________AddNode");

        // var newNode = await context.Nodes.SingleOrDefaultAsync(n =>
        //     n.NodeUniqueString == (string)record[uniqueHeaderName]
        //     && n.NodeCategoryId == nodeCategoryId);
        // if (newNode == null)
        // {
            var newNode = new Models.Graph.Node()
            {
                NodeUniqueString = (string)record[uniqueHeaderName],
                NodeCategoryId = nodeCategoryId,
                NodeId = ++context.LastNodeId
            };
            var t2 = Stopwatch.StartNew();
            await context.Nodes.AddAsync(newNode);
            //await context.SaveChangesAsync();
            t2.Stop();
            Console.WriteLine(t2.ElapsedMilliseconds.ToString()+"%%%%%%%%%%%%%%%%%");
            
        //} 

        // foreach (var kvp in record)
        // {
        //     if (kvp.Key != uniqueHeaderName)
        //     {
        //         var t3 = Stopwatch.StartNew();
        //         
        //         var newNodeAttribute = context.NodeAttributes.SingleOrDefault(na => na.NodeAttributeName == kvp.Key);
        //         // if (newNodeAttribute == null)
        //         // {
        //         //     newNodeAttribute = new NodeAttribute()
        //         //     {
        //         //         NodeAttributeId = ++context.LastAttributeId,
        //         //         NodeAttributeName = kvp.Key
        //         //     };
        //         //     await context.NodeAttributes.AddAsync(newNodeAttribute);
        //         //     //await context.SaveChangesAsync();
        //         // }
        //         
        //         t3.Stop();
        //         Console.WriteLine(t3.ElapsedMilliseconds.ToString()+"???????");
        //
        //         // var t5 = Stopwatch.StartNew();
        //         // var value = await context.NodeValues.SingleOrDefaultAsync(nv =>
        //         //     nv.NodeAttributeId == newNodeAttribute.NodeAttributeId &&
        //         //     nv.NodeId == newNode.NodeId);
        //         //
        //         // if (value != null)
        //         // {
        //         //     throw new Exception(Resources.FailedAddRecordsMessage);
        //         // }
        //         // t5.Stop();
        //         // Console.WriteLine(t5.ElapsedMilliseconds.ToString()+"@@@@@@@@@@@");
        //         
        //         var newNodeValue = new NodeValue()
        //         {
        //             NodeAttributeId = newNodeAttribute.NodeAttributeId,
        //             ValueData = kvp.Value.ToString(),
        //             NodeId = newNode.NodeId
        //         };
        //
        //         var t4 = Stopwatch.StartNew();
        //         
        //         await context.NodeValues.AddAsync(newNodeValue);
        //         //await context.SaveChangesAsync();
        //         t4.Stop();
        //         Console.WriteLine(t4.ElapsedMilliseconds.ToString()+"*************");
        //         
        //     }
        // }
        t.Stop();
        Console.WriteLine(t.ElapsedMilliseconds.ToString()+"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
    }
}