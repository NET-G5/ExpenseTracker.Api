using ExpenseTracker.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Route("api/root")] 
[ApiController]
public class RootController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Link>> GetRoot()
    {
        var categoriesUrl = Url.Action(nameof(CategoriesController.GetOptions), "Categories");
        var categoriesLink = new Link(categoriesUrl, "LINKS", "OPTIONS");

        var walletsUrl = Url.Action(nameof(WalletsController.GetOptions), "Wallets");
        var walletsLink = new Link(walletsUrl, "LINKS", "OPTIONS");

        var transfersUrl = Url.Action(nameof(TransfersController.GetOptions), "Transfers");
        var transfersLink = new Link(transfersUrl, "LINKS", "OPTIONS");

        var urls = new List<Link>
        {
            categoriesLink,
            categoriesLink,
            walletsLink
        };

        return Ok(urls);
    }

   


}
