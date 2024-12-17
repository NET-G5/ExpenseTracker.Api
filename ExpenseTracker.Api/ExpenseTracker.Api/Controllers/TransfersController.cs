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

    /// <summary>
    /// Get a list of transfers.
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering and sorting.</param>
    /// <returns> Paginated list of transfers.</returns>
    [HttpHead]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PaginatedResponse<TransferDto>>> GetAsync([FromQuery] TransferQueryParameters queryParameters)
    {
        var transfers = await _transferService.GetAsync(queryParameters);

        var metadataJson = JsonConvert.SerializeObject(transfers.PaginationMetadata);
        HttpContext.Response.Headers.Append("X-Pagination", metadataJson);

        return Ok(transfers.Data);
    }

    /// <summary>
    /// Get a transfer by ID
    /// </summary>
    /// <param name="request">The Transfer ID</param>
    /// <returns>A single transfer</returns>
    [HttpGet("{id:int:min(1)}", Name = nameof(GetTransferByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TransferDto>> GetTransferByIdAsync([FromRoute] TransferRequest request)
    {
        var transfer = await _transferService.GetByIdAsync(request);

        return Ok(transfer);
    }
    /// <summary>
    /// Create a new transfer.
    /// </summary>
    /// <param name="request">Transfer to create </param>
    /// <returns> Newly created transfer.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TransferDto>> CreateAsync([FromBody] CreateTransferRequest request)
    {
        var transfer = await _transferService.CreateAsync(request);

        return CreatedAtAction(nameof(GetTransferByIdAsync), new { id = transfer.Id }, transfer);
    }
    /// <summary>
    /// Updates Transfer
    /// </summary>
    /// <param name="id">transfer Id</param>
    /// <param name="request">Transfer to update.</param>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    /// <summary>
    /// Deletes a single Transfer
    /// </summary>
    /// <param name="id">transfer Id</param>
    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] int id)
    {
        var transferRequest = new TransferRequest(id);

        await _transferService.DeleteAsync(transferRequest);

        return NoContent();
    }

    /// <summary>
    /// Get allowed methods for this resource. 
    /// </summary>
    /// <returns>Allowed methods for this resource.</returns>
    [HttpOptions]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetOptions()
    {
        string[] options = ["GET", "POST", "PUT", "DELETE", "HEAD"];

        HttpContext.Response.Headers.Append("X-Options", JsonConvert.SerializeObject(options));

        return Ok(options);
    }
}
