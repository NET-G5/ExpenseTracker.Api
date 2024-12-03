using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Transfer;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Route("api/transfers")]
[ApiController]
public class TransferController : ControllerBase
{
    private readonly ITranferService _transferService;

    public TransferController(ITranferService transferService)
    {
        _transferService = transferService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransferDto>>> GetAsync([FromQuery] QueryParametersBase queryParameters)
    {
        var transfers = await _transferService.GetAsync(queryParameters);

        return Ok(transfers);
    }
    
    [HttpGet("{id:int:min(1)}", Name = nameof(GetTransferByIdAsync))]
    public async Task<ActionResult<TransferDto>> GetTransferByIdAsync([FromRoute] TransferRequest request)
    {
        var transfer = await _transferService.GetByIdAsync(request);

        return Ok(transfer);
    }

    [HttpPost]
    public async Task<ActionResult<TransferDto>> CreateAsync([FromBody] CreateTranferRequest request)
    {
        var transfer = await _transferService.CreateAsync(request);

        return CreatedAtAction(nameof(GetTransferByIdAsync), new { id = transfer.Id }, transfer);
    }

    [HttpPut("{id:int:min(1)}")]
    public async Task<ActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] UpdateTranferRequest request)
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
