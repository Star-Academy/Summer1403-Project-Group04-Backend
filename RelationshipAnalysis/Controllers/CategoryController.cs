using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto.Category;

namespace RelationshipAnalysis.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class CategoryController : ControllerBase
{


    [HttpGet]
    public async Task<IActionResult> GetAllNodeCategories()
    {
        //...
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllEdgeCategories()
    {
        //...
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateNodeCategory([FromBody] CreateNodeCategoryDto createNodeCategoryDto)
    {
        //...
        return Ok();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateEdgeCategory([FromBody] CreateEdgeCategoryDto createEdgeCategoryDto)
    {
        //...
        return Ok();
    }
}