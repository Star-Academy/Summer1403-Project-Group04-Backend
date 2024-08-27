using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto.Graph;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GraphController(IGraphReceiver graphReceiver,
    IExpansionGraphReceiver expansionGraphReceiver) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetGraph()
    {
        return Ok(await graphReceiver.GetGraph());
    }
    [HttpGet("expansion")]
    public async Task<IActionResult> GetExpansionGraph(int nodeId, string sourceCategoryName, string targetCategoryName, string edgeCategoryName)
    {
        var result = await expansionGraphReceiver.GetExpansionGraph(nodeId, sourceCategoryName, targetCategoryName, edgeCategoryName);
        return Ok(result);
    }
}