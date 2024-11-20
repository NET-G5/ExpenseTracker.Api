using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.QueryParameters;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ExpenseTracker.Api.Controllers;

[ApiController]
[Route("api/transfers")]
public class TransfersController : ControllerBase
{
    private readonly ITransferService _transfersService;

    public TransfersController(ITransferService transferService)
    {
        _transfersService = transferService;
    }

    [HttpGet]
    [HttpHead]
    public async Task<ActionResult<List<Transfer>>> GetAll(TransferFilter? filter = null)
    {
        var transfers = await _transfersService.GetAsync(filter);

        return Ok(transfers);
    }

    [HttpGet("{id:int:min(1):max(500)}", Name = "GetTransferById")]
    public async Task<ActionResult<ResultWithLinks<Transfer>>> GetById([FromRoute]int id)
    {
        var transfer = await _transfersService.GetByIdAsync(id);
        var result = new ResultWithLinks<Transfer>(transfer);

        AddLinks(result);
        Response.Headers.Append("links", JsonSerializer.Serialize(result.Links));

        return Ok(result.Data);
    }

    [HttpPost("create")]
    public async Task<ActionResult<ResultWithLinks<Transfer>>> Create([FromBody] Transfer transfer)
    {
       var createdTransfer = await _transfersService.CreateAsync(transfer);

        return CreatedAtRoute("GetTransferById", new { id = createdTransfer.Id }, createdTransfer);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] Transfer transfer)
    {
        if (id != transfer.Id)
        {
            return BadRequest($"Route id: {id} does not match with body id: {transfer.Id}");
        }

        await _transfersService.UpdateAsync(transfer);

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _transfersService.DeleteAsync(id);

        return NoContent();
    }


    [HttpOptions]
    public ActionResult<List<string>> GetOptions()
    {
        var options = new List<string> { "GET", "POST", "PUT", "DELETE", "HEAD" };

        Response.Headers.AppendList("X-Options", options);
        return Ok(options);
    }

    private void AddLinks(ResultWithLinks<Transfer> result)
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
    }
}
