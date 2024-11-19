using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.QueryParameters;
using ExpenseTracker.Api.Services;
using ExpenseTracker.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ExpenseTracker.Api.Controllers;

[Route("api/wallets")]
[ApiController]
public class WalletsController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ITransferService _transferService;

    public WalletsController(IWalletService walletService, ITransferService transferService)
    {
        this._walletService = walletService;
        this._transferService = transferService;
    }


    [HttpGet]
    [HttpHead]
    public async Task<ActionResult<List<Wallet>>> GetAll([FromQuery] WalletFilter filter)
    {
        var wallets = await _walletService.GetAsync(filter);

        return Ok(wallets);
    }
    [HttpGet("{id:int:min(1):max(500)}", Name = "GetWalletById")]
    public async Task<ActionResult<ResultWithLinks<Wallet>>> GetById([FromRoute] int id)
    {
        var wallet = await _walletService.GetByIdAsync(id);
        var result = new ResultWithLinks<Wallet>(wallet);

        AddLinks(result);
        Response.Headers.Append("links", JsonSerializer.Serialize(result.Links));

        return Ok(result.Data);
    }

    [HttpGet("{id}/transfers")]
    public async Task<ActionResult<List<Transfer>>> GetTransfers([FromRoute] int id)
    {
        var transfers = await _transferService.GetAsync(new TransferFilter { CategoryId = 0 });

        return Ok(transfers);
    }

    [HttpPost]
    public async Task<ActionResult<ResultWithLinks<Wallet>>> Create([FromBody] Wallet wallet)
    {
        var createdWallet = await _walletService.CreateAsync(wallet);

        return CreatedAtRoute("GetCategoryById", new { id = createdWallet.Id }, createdWallet);
    }

    [HttpOptions]
    public ActionResult<List<string>> GetOptions()
    {
        var options = new List<string> { "GET", "POST", "PUT", "DELETE" };

        Response.Headers.AppendList("options", options);
        return Ok(options);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromRoute] int id, [FromBody] Wallet wallet)
    {
        if (id != wallet.Id)
        {
            return BadRequest($"Route id: {id} does not match with body id: {wallet.Id}");
        }

        await _walletService.UpdateAsync(wallet);

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _walletService.DeleteAsync(id);

        return NoContent();
    }
    private void AddLinks(ResultWithLinks<Wallet> result)
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
