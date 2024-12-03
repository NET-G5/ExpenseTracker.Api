using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExpenseTracker.Api.Controllers;

[Authorize]
[Route("api/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [HttpHead]
    public async Task<ActionResult<List<CategoryDto>>> GetAsync([FromQuery] QueryParametersBase queryParameters)
    {
        var categories = await _categoryService.GetAsync(queryParameters);

        return Ok(categories);
    }

    [HttpGet("{id:int:min(1)}", Name = nameof(GetCategoryByIdAsync))]
    public async Task<ActionResult<CategoryDto>> GetCategoryByIdAsync([FromRoute] CategoryRequest request)
    {
        var category = await _categoryService.GetByIdAsync(request);

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateAsync([FromBody] CreateCategoryRequest request)
    {
        var response = await _categoryService.CreateAsync(request);

        return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> PutAsync(
        [FromRoute] int id, 
        [FromBody] UpdateCategoryRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest($"Route parameter does not match with body parameter: {request.Id}");
        }

        await _categoryService.UpdateAsync(request);

        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] CategoryRequest request)
    {
        await _categoryService.DeleteAsync(request);

        return NoContent();
    }

    [HttpOptions]
    public IActionResult GetOptions()
    {
        string[] options = ["GET", "POST", "PUT", "DELETE", "HEAD","PATCH"];

        HttpContext.Response.Headers.Append("X-Options", JsonConvert.SerializeObject(options));

        return Ok(options);
    }
}
