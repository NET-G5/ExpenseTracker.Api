using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Models;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Transfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExpenseTracker.Api.Controllers;

[Authorize]
[Route("api/transfers")]
[ApiController]
public class TransferController : ControllerBase
{
    private readonly ITransferService _transferService;

    public TransferController(ITransferService transferService)
    {
        
        _transferService = transferService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<TransferDto>>> GetAsync([FromQuery] TransferQueryParameters queryParameters)
    {
        var transfers = await _transferService.GetAsync(queryParameters);

        var metadataJson = JsonConvert.SerializeObject(transfers.PaginationMetadata);
        HttpContext.Response.Headers.Append("X-Pagination", metadataJson);

        return Ok(transfers.Data);
    }

    [HttpGet("{id:int:min(1)}", Name = nameof(GetTransferByIdAsync))]
    public async Task<ActionResult<TransferDto>> GetTransferByIdAsync([FromRoute] TransferRequest request)
    {
        var transfer = await _transferService.GetByIdAsync(request);

        return Ok(transfer);
    }

    [HttpPost]
    public async Task<ActionResult<TransferDto>> CreateAsync([FromBody] CreateTransferRequest request)
    {
        var transfer = await _transferService.CreateAsync(request);

        return CreatedAtAction(nameof(GetTransferByIdAsync), new { id = transfer.Id }, transfer);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] UpdateTransferRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest($"Route parameter does not match with body parameter: {request.Id}");
        }

        await _transferService.UpdateAsync(request);

        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        var transferRequest = new TransferRequest(id);

        await _transferService.DeleteAsync(transferRequest);

        return NoContent();
    }
}
