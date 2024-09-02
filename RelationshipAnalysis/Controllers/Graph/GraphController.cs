using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Services.GraphServices.Abstraction;
using RelationshipAnalysis.Services.GraphServices.Graph.Abstraction;

namespace RelationshipAnalysis.Controllers.Graph;

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
    public async Task<IActionResult> GetExpansionGraph([FromQuery] int nodeId,[FromQuery] string sourceCategoryName,[FromQuery] string targetCategoryName,[FromQuery] string edgeCategoryName)
    {
        var result = await expansionGraphReceiver.GetExpansionGraph(nodeId, sourceCategoryName, targetCategoryName, edgeCategoryName);
        return Ok(result);
    }
}