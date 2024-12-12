using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.QueryParameters;
using ExpenseTracker.Application.Requests.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public async Task<ActionResult<List<WalletDto>>> GetAsync([FromQuery] WalletQueryParameters queryParameters)
    {
        var wallets = await _walletService.GetAsync(queryParameters);

        return Ok(wallets);
    }

    [HttpGet("{id:int:min(1)}", Name = nameof(GetWalletByIdAsync))]
    public async Task<ActionResult<WalletDto>> GetWalletByIdAsync([FromRoute] WalletRequest request)
    {
        var wallet = await _walletService.GetByIdAsync(request);

        return Ok(wallet);
    }

    [HttpPost]
    public async Task<ActionResult<WalletDto>> CreateAsync([FromBody] CreateWalletRequest request)
    {
        var response = await _walletService.CreateAsync(request);

        return CreatedAtAction(nameof(GetWalletByIdAsync), new { id = response.Id }, response);
    }

    [HttpPut("{id:int:min(1)}")]
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

    [HttpDelete("{walletId:int:min(1)}")]
    public async Task<ActionResult> DeleteAsync([FromRoute] WalletRequest request)
    {
        await _walletService.DeleteAsync(request);

        return NoContent();
    }
}
