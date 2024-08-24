using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Services.GraphServices.Abstraction;

namespace RelationshipAnalysis.Controllers.Graph;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GraphController(IGraphReceiver graphReceiver) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetGraph()
    {
        return Ok(await graphReceiver.GetGraph());
    }
}