using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ExpenseTracker.Api.Controllers;

[Route("api/wallets")]
[ApiController]
[Authorize]
public class WalletsController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletsController(IWalletService walletService)
    {
        _walletService = walletService;
    }
    /// <summary>
    /// Get a List of Wallets.
    /// </summary>
    /// <param name="queryParameters">Query parameters for filtering and sorting.</param>
    /// <returns>List of wallets.</returns>
    [HttpGet]
    [HttpHead]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<WalletDto>>> GetAsync([FromQuery] WalletQueryParameters queryParameters)
    {
        var wallets = await _walletService.GetAsync(queryParameters);

        return Ok(wallets);
    }
    /// <summary>
    /// Get a wallet by ID.
    /// </summary>
    /// <param name="request">Wallet ID</param>
    /// <returns>A single wallet.</returns>
    [HttpGet("{id:int:min(1)}", Name = nameof(GetWalletByIdAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WalletDto>> GetWalletByIdAsync([FromRoute] WalletRequest request)
    {
        var wallet = await _walletService.GetByIdAsync(request);

        return Ok(wallet);
    }

    /// <summary>
    /// Create a new Wallet.
    /// </summary>
    /// <param name="request">Wallet to create.</param>
    /// <returns>Newly created Wallet</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<WalletDto>> CreateAsync([FromBody] CreateWalletRequest request)
    {
        var response = await _walletService.CreateAsync(request);

        return CreatedAtAction(nameof(GetWalletByIdAsync), new { id = response.Id }, response);
    }

    /// <summary>
    /// Update wallet.
    /// </summary>
    /// <param name="id">wallet ID</param>
    /// <param name="request">Wallet to update.</param>
    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> PutAsync(
        [FromRoute] int id,
        [FromBody] UpdateWalletRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest($"Route parameter does not match with body parameter: {request.Id}");
        }

        await _walletService.UpdateAsync(request);

        return NoContent();
    }
    /// <summary>
    /// Deletes a single Wallet
    /// </summary>
    /// <param name="request">Wallet ID to delete </param>
    [HttpDelete("{walletId:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] WalletRequest request)
    {
        await _walletService.DeleteAsync(request);

        return NoContent();
    }

    /// <summary>
    ///  Gets allowed methods for this resource.
    /// </summary>
    /// <returns>Allowed methods for this resource</returns>
    [HttpOptions]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status406NotAcceptable)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetOptions()
    {
        string[] options = ["HEAD", "GET", "POST", "PUT", "DELETE"];

        HttpContext.Response.Headers.Append("X-Options", JsonConvert.SerializeObject(options));

        return Ok(options);
    }
}
