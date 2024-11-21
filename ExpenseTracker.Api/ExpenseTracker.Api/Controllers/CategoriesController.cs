using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Category;
using ExpenseTracker.Application.Requests.Common;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ExpenseTracker.Api.Controllers;

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
    public async Task<ActionResult<List<CategoryDto>>> GetAsync(
        [FromBody] UserRequest request,
        [FromQuery] QueryParametersBase queryParameters)
    {
        var categories = await _categoryService.GetAsync(request, queryParameters);

        return Ok(categories);
    }

    [HttpGet("{id:int:min(1)}", Name = nameof(GetCategoryByIdAsync))]
    public async Task<ActionResult<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Post([FromBody] CreateCategoryRequest request)
    {
        var response = await _categoryService.CreateAsync(request);

        return CreatedAtAction(nameof(GetCategoryByIdAsync), response, new { id = response.Id });
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

    [HttpDelete("{categoryId:int:min(1)}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] CategoryRequest request)
    {
        await _categoryService.DeleteAsync(request);

        return NoContent();
    }
}
