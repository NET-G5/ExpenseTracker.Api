using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.QueryParameters;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExpenseTracker.Api.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly ITransferService _transferSerivice;

    public CategoriesController(ICategoryService categoryService, ITransferService transfers)
    {
        _categoryService = categoryService;
        _transferSerivice = transfers;
    }

    [HttpGet]
    [HttpHead]
    public async Task<ActionResult<List<Category>>> GetAll([FromQuery] CategoryFilter filter)
    {
        var categories = await _categoryService.GetAsync(filter);

        return Ok(categories);
    }

    [HttpGet("{id:int:min(1):max(500)}", Name = "GetCategoryById")]
    public async Task<ActionResult<ResultWithLinks<Category>>> GetById([FromRoute] int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        var result = new ResultWithLinks<Category>(category);

        AddLinks(result);
        Response.Headers.Append("links", JsonSerializer.Serialize(result.Links));

        return Ok(result.Data);
    }

    [HttpGet("{id}/transfers")]
    public async Task<ActionResult<List<Transfer>>> GetTransfers([FromRoute] int id)
    {
        var transfers = await _transferSerivice.GetAsync(new TransferFilter { CategoryId = 0 });

        return Ok(transfers);
    }

    [HttpPost]
    public async Task<ActionResult<ResultWithLinks<Category>>> Create([FromBody] Category category)
    {
        var createdCategory = await _categoryService.CreateAsync(category);

        return CreatedAtRoute("GetCategoryById", new { id = createdCategory.Id }, createdCategory);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] Category category)
    {
        if (id != category.Id)
        {
            return BadRequest($"Route id: {id} does not match with body id: {category.Id}");
        }

        await _categoryService.UpdateAsync(category);

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _categoryService.DeleteAsync(id);

        return NoContent();
    }

    [HttpOptions]
    public ActionResult<List<string>> GetOptions()
    {
        var options = new List<string> { "GET", "POST", "PUT", "DELETE" };

        Response.Headers.AppendList("options", options);
        return Ok(options);
    }

    private void AddLinks(ResultWithLinks<Category> result)
    {
        var deleteUrl = Url.Action("Delete", new { result.Data.Id });

        if (deleteUrl != null)
        {
            var link = new Link(deleteUrl, "delete", "DELETE");
            result.AddLink(link);
        }

        var updateUrl = Url.Action("Update", new { result.Data.Id });

        if (updateUrl != null)
        {
            var link = new Link(updateUrl, "update", "PUT");
            result.AddLink(link);
        }

        var getUrl = Url.Action("GetById", new { result.Data.Id });

        if (getUrl != null)
        {
            var link = new Link(getUrl, "self", "GET");
            result.AddLink(link);
        }

        var childrenUrl = Url.Action(nameof(GetTransfers), new { result.Data.Id });

        if (childrenUrl != null)
        {
            var link = new Link(childrenUrl, "transfers", "GET");
            result.AddLink(link);
        }
    }
}