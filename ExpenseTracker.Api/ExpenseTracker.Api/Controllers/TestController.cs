
using ExpenseTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers;

[Route("api/test")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("get")]
    public Category GetNumber()
    {
        return new Category { Id = 1, Name = "Test", Type = CategoryType.Expense};
    }

    [HttpGet("get1")]
    public IActionResult GetIActionResult()
    {
        return StatusCode(200, new Category { Id = 1, Name = "Test", Type = CategoryType.Expense });
    }

    [HttpGet("get2")]
    public ActionResult<Category> GetActionResult()
    {
        return StatusCode(200, new Transfer { Amount = 0} );
    }
}

// RESTFul API 6 Constraints
// Richardson maturity level
// REST API History
// REST vs SOAP vs gRPC vs GraphQL
// ActionResult vs IActionResult vs Concrete return type
// What is Resource? What is HATEOAS?
// REQUEST, RESPONSE, URL/URI, HEADER, Params (BODY, QUERY, URL, HEADER)

