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
    private readonly ITransferService _transferService;

    public CategoriesController(ICategoryService categoryService, ITransferService transferService)
    {
        _categoryService = categoryService;
        _transferService = transferService;
    }

    /// <summary>
    /// Gets a list of Categories.
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering and sorting.</param>
    /// <returns>List of categories.</returns>
    [HttpGet]
    [HttpHead]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<CategoryDto>>> GetAsync([FromQuery] CategoryQueryParameters queryParameters)
    {
        var categories = await _categoryService.GetAsync(queryParameters);

        return Ok(categories);
    }

    /// <summary>
    /// Gets a category by ID.
    /// </summary>
    /// <param name="request">The Category Id</param>
    /// <returns>A single category.</returns>
    [HttpGet("{id:int:min(1)}", Name = nameof(GetCategoryByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> GetCategoryByIdAsync([FromRoute] CategoryRequest request)
    {
        var category = await _categoryService.GetByIdAsync(request);

        return Ok(category);
    }

    /// <summary>
    /// Gets a list of transfers for given category.
    /// </summary>
    /// <param name="request">Category ID for which transfer belong to.</param>
    /// <returns>List of transfers</returns>
    [HttpGet("{id:int:min(1)}/transfers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TransferDto>> GetTransfersAsync([FromRoute] CategoryRequest request)
    {
        var transfers = await _transferService.GetByCategoryAsync(request);

        return Ok(transfers);
    }

    /// <summary>
    /// Creates a new Category.
    /// </summary>
    /// <param name="request">Category to create.</param>
    /// <returns>Newly created category.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CategoryDto>> CreateAsync([FromBody] CreateCategoryRequest request)
    {
        var response = await _categoryService.CreateAsync(request);

        return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = response.Id }, response);
    }

    /// <summary>
    /// Updates Category.
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="request">Category to update.</param>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Deletes a single Category.
    /// </summary>
    /// <param name="request">Category Id to delete.</param>
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] CategoryRequest request)
    {
        await _categoryService.DeleteAsync(request);

        return NoContent();
    }

    /// <summary>
    /// Gets allowed methods for this resource.
    /// </summary>
    /// <returns>Allowed methods for this resource</returns>
    [HttpOptions]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetOptions()
    {
        string[] options = ["GET", "POST", "PUT", "DELETE", "HEAD", "PATCH"];

        HttpContext.Response.Headers.Append("X-Options", JsonConvert.SerializeObject(options));

        return Ok(options);
    }
}
